using DAL.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesDAO _servicesDAO;

        public ServicesController(IServicesDAO servicesDAO)
        {
            _servicesDAO = servicesDAO;
        }


        [HttpGet]
        public ActionResult<IEnumerable<ServicesEntity>> Get()
        {
            return Ok(_servicesDAO.Select());
        }

        [HttpGet("Provider/{providerID:guid}")]
        public ActionResult<ServicesEntity> GetByProviderID(Guid providerID)
        {
            if (providerID != Guid.Empty)
            {
                var service = _servicesDAO.SelectProviderID(providerID);
                if (service != null)
                {
                    return Ok(service);
                }
                return NotFound("Service not found");
            }
            return BadRequest("Invalid provider identifier");
        }

        [HttpGet("Search")]
        public ActionResult<IEnumerable<ServicesEntity>> SearchByName([FromQuery] string serviceName)
        {
            if (!string.IsNullOrEmpty(serviceName))
            {
                var services = _servicesDAO.GetServicesByName(serviceName);
                if (services.Any())
                {
                    return Ok(services);
                }
                return Ok(new List<ServicesEntity>()); ;
            }
            return BadRequest("Service name cannot be null or empty.");
        }

        [HttpGet("Category/{categoryID:guid}")]
        public ActionResult<ServicesEntity> GetByCategoryID(Guid categoryID)
        {
            if (categoryID != Guid.Empty)
            {
                var service = _servicesDAO.SelectCategoryID(categoryID);
                if (service != null)
                {
                    return Ok(service);
                }
                return NotFound("Service not found");
            }
            return BadRequest("Invalid category identifier");
        }

        [HttpGet("{serviceID:guid}")]
        public ActionResult<ServicesEntity> GetByServiceID(Guid serviceID)
        {
            if (serviceID != Guid.Empty)
            {
                var service = _servicesDAO.SelectServiceID(serviceID);
                if (service != null)
                {
                    return Ok(service);
                }
                return NotFound("Service not found");
            }
            return BadRequest("Invalid service identifier");
        }

        [HttpGet("DTO/{serviceID:guid}")]
        public ActionResult<ServiceDTO> SelectDTOByID(Guid serviceID)
        {
            if (serviceID != Guid.Empty)
            {
                var service = _servicesDAO.SelectDTOByID(serviceID);
                if (service != null)
                {
                    return Ok(service);
                }
                return NotFound("Service not found");
            }
            return BadRequest("Invalid service identifier");
        }

        [HttpPost]
        public ActionResult<ServicesEntity> Post(ServicesEntity service)
        {
            if (service == null || service.ProviderID == Guid.Empty)
                return BadRequest("Invalid service entity or serviceID.");

            _servicesDAO.Insert(service);

            return CreatedAtAction(nameof(Get), new { id = service.ProviderID }, service);
        }

        [HttpPut]
        public ActionResult<ServicesEntity> Put(ServicesEntity service)
        {
            if (service == null || service.ProviderID == Guid.Empty)
                return BadRequest("Invalid service entity or serviceID.");

            _servicesDAO.Update(service);

            return CreatedAtAction(nameof(Get), new { id = service.ProviderID }, service);
        }

        [HttpDelete("{serviceID:guid}")]
        public ActionResult Delete(Guid serviceID)
        {
            if (serviceID == Guid.Empty)
                return BadRequest("Invalid ServiceID.");

            _servicesDAO.Delete(serviceID);
            return NoContent();
        }
    }
}
