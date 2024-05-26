using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IMapper _mapper;

        public DepartmentController(
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            ILogger<DepartmentController> logger,
            IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            //_departmentRepository = departmentRepository;
            _logger = logger;
            _mapper = mapper;
        }
        
        public IActionResult Index(string SearchValue = "")
        {
            IEnumerable<Department> departments;
            IEnumerable<DepartmentViewModel> departmentsViewModel;

            if (string.IsNullOrEmpty(SearchValue))
            {
                departments = _unitOfWork.DepartmentRepository.GetAll();
                departmentsViewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);
            }
            else 
            {
                departments = _unitOfWork.DepartmentRepository.Search(SearchValue);
                departmentsViewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);
            }

            //ViewBag.MessageData = "Hello From View Bag!";
            //ViewData["Message"] = "Hello From View Data!";
            TempData.Keep("MessageDate");

            return View(departmentsViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }
        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                //Department department = new Department
                //{
                //    Name = departmentViewModel.Name,
                //    Code = departmentViewModel.Code,
                //    CreateAt = departmentViewModel.CreateAt
                //};

                var department = _mapper.Map<Department>(departmentViewModel);

                _unitOfWork.DepartmentRepository.Add(department);
                _unitOfWork.Complete();
                TempData["MessageDate"] = "Department Added Successfully!";
                //return RedirectToAction("Index");
                return RedirectToAction(nameof(Index));
            }
            return View(departmentViewModel);
        }
        public IActionResult Details(int? id)
        {
            try
            {
                if (id is null)
                    return BadRequest();
                var department = _unitOfWork.DepartmentRepository.GetById(id);
                if (department is null)
                    return NotFound();
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Update(int? id)
        {
            if (id is null)
                return BadRequest();

            var department = _unitOfWork.DepartmentRepository.GetById(id);

            if (department is null)
                return NotFound();

            return View(department);
        }
        [HttpPost]
        public IActionResult Update(int? id, Department department)
        {
            if (id != department.Id)
                return BadRequest();

            try
            {
                if (ModelState.IsValid)
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return View(department);
        }
        public IActionResult Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var department = _unitOfWork.DepartmentRepository.GetById(id);

            if (department is null)
                return NotFound();

            _unitOfWork.DepartmentRepository.Delete(department);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
