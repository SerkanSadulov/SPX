using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsEntityController : ControllerBase
    {
        private readonly IRatingsDAO _ratingsEntityDAO;

        public RatingsEntityController(IRatingsDAO ratingsEntityDAO)
        {
            _ratingsEntityDAO = ratingsEntityDAO;
        }


        [HttpGet]
        public ActionResult<IEnumerable<RatingsEntity>> Get()
        {
            return Ok(_ratingsEntityDAO.Select());
        }


        [HttpGet("HighestRated")]
        public ActionResult<IEnumerable<RatingsEntity>> GetHighestRated()
        {
            return Ok(_ratingsEntityDAO.SelectHighestRated());
        }


        [HttpGet("{serviceId:guid}/{clientId:guid}")]
        public ActionResult<IEnumerable<RatingsEntity>> Get(Guid serviceId, Guid clientId)
        {
            if (serviceId != Guid.Empty && clientId != Guid.Empty)
            {
                var ratingsEntities = _ratingsEntityDAO.Select(serviceId, clientId);
                if (ratingsEntities.Any())
                {
                    return Ok(ratingsEntities);
                }
                return NotFound("Service rating not found");
            }
            return BadRequest("Invalid service or client identifier");
        }

        [HttpGet("{serviceId:guid}")]
        public ActionResult<IEnumerable<RatingsEntity>> GetByServiceId(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
            {
                return BadRequest("Invalid ServiceID");
            }

            var ratingsEntities = _ratingsEntityDAO.SelectByServiceID(serviceId);
            if (ratingsEntities == null || !ratingsEntities.Any())
            {
                return Ok(new List<RatingsEntity>()); ;
            }

            return Ok(ratingsEntities);
        }

        [HttpPost]
        public ActionResult<RatingsEntity> Post(RatingsEntity ratingsEntity)
        {
            if (ratingsEntity == null || ratingsEntity.ServiceID == Guid.Empty || ratingsEntity.ClientID == Guid.Empty)
                return BadRequest("Invalid RatingsEntity entity or IDs.");

            _ratingsEntityDAO.Insert(ratingsEntity);

            return CreatedAtAction(nameof(Get), new { serviceId = ratingsEntity.ServiceID, clientId = ratingsEntity.ClientID }, ratingsEntity);
        }

        [HttpPut]
        public ActionResult<RatingsEntity> Put(RatingsEntity ratingsEntity)
        {
            if (ratingsEntity == null || ratingsEntity.ServiceID == Guid.Empty || ratingsEntity.ClientID == Guid.Empty)
                return BadRequest("Invalid RatingsEntity entity or IDs.");

            _ratingsEntityDAO.Update(ratingsEntity);

            return CreatedAtAction(nameof(Get), new { serviceId = ratingsEntity.ServiceID, clientId = ratingsEntity.ClientID }, ratingsEntity);
        }

        [HttpDelete("{serviceId:guid}/{clientId:guid}")]
        public ActionResult Delete(Guid serviceId, Guid clientId)
        {
            if (serviceId == Guid.Empty || clientId == Guid.Empty)
                return BadRequest("Invalid ServiceID or ClientID.");

            _ratingsEntityDAO.Delete(serviceId, clientId);
            return NoContent();
        }
    }
}
