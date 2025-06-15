using DAL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesDAO _categoriesDAO;

        public CategoriesController(ICategoriesDAO categoriesDAO)
        {
            _categoriesDAO = categoriesDAO;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriesEntity>> Get()
        {
            var categories = _categoriesDAO.Select();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<CategoriesEntity> Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid category identifier");
            }

            var category = _categoriesDAO.Select(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpPost]
        public ActionResult<CategoriesEntity> Post([FromBody] CategoriesEntity category)
        {
            if (category == null || category.CategoryID == Guid.Empty)
            {
                return BadRequest("Invalid Category entity or CategoryId.");
            }

            _categoriesDAO.Insert(category);

            return CreatedAtAction(nameof(Get), new { id = category.CategoryID }, category);
        }
    }
}
