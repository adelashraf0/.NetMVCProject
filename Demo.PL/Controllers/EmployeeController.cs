using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;
        

        public EmployeeController(
            IUnitOfWork unitOfWork,
            ILogger<EmployeeController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
       
        public IActionResult Index(string SearchValue = "")
        {
            IEnumerable<Employee> employees;
            IEnumerable<EmployeeViewModel> employeesViewModel;

            if (string.IsNullOrEmpty(SearchValue))
            {
                employees = _unitOfWork.EmployeeRepository.GetAll();
                employeesViewModel = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }
            else
            {
                employees = _unitOfWork.EmployeeRepository.Search(SearchValue);
                employeesViewModel = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }

            return View(employeesViewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();
            return View(new EmployeeViewModel());
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel employeeViewModel)
        {
            //ModelState["Department"].ValidationState = ModelValidationState.Valid;
            if(ModelState.IsValid) 
            {
                // Manual Mapping
                //Employee employee = new Employee
                //{
                //    Name = employeeViewModel.Name,
                //    Email = employeeViewModel.Email,
                //    Address = employeeViewModel.Address,
                //    HireDate = employeeViewModel.HireDate,
                //    Salary = employeeViewModel.Salary,
                //    DepartmentId = employeeViewModel.DepartmentId,
                //    IsActive = employeeViewModel.IsActive
                //};

                //var employee = _mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                var employee = _mapper.Map<Employee>(employeeViewModel);
                employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");

                _unitOfWork.EmployeeRepository.Add(employee);
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            return View(employeeViewModel);
        }
        public IActionResult Details(int? id) 
        {
            try
            {
                if (id is null)
                    return BadRequest();

                var employee = _unitOfWork.EmployeeRepository.GetById(id);
                if (employee is null)
                    return NotFound();
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id is null)
                return BadRequest();

            var employee = _unitOfWork.EmployeeRepository.GetById(id);
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            if (employee is null)
                return NotFound();
            var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);
            return View(employeeViewModel);
        }
        [HttpPost]
        public IActionResult Update(int? id, EmployeeViewModel employeeViewModel)
        {
            //ModelState["Department"].ValidationState = ModelValidationState.Valid;
            if (id != employeeViewModel.Id)
                return BadRequest();

            try
            {
                if (ModelState.IsValid)
                {
                    var employee = _mapper.Map<Employee>(employeeViewModel);
                    employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");
                    _unitOfWork.EmployeeRepository.Update(employee);
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll();

            return View(employeeViewModel);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            var employee = _unitOfWork.EmployeeRepository.GetById(id);

            if (employee is null)
                return NotFound();
            
            _unitOfWork.EmployeeRepository.Delete(employee);
            _unitOfWork.Complete();
            return RedirectToAction(nameof(Index));
        }
    }
}
