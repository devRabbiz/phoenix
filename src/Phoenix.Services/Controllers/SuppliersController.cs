using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Phoenix.Domain.Suppliers;

namespace Phoenix.Services.Controllers
{
    [Route("api/suppliers")]
    public class SuppliersController : PhoenixBaseController
    {
        private readonly ISupplierService _service;
        private readonly IMapper _mapper;
        public SuppliersController(ISupplierService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SupplierListDto> GetAll(int page = 1, int size = 10)
        {
            try
            {
                var suppliers = _service.GetAllSuppliers(page, size);
                int total = _service.TotalSuppliers;
                string next = string.Empty;
                
                if (page * size < total )
                    next = Url.Action(nameof(GetAll), new { page = page + 1, size = size });

                var data = _mapper.Map<IEnumerable<Supplier>, IEnumerable<SupplierDto>>(suppliers);

                var response = new SupplierListDto { Data = data, Next = next, Total = total };
                return Ok(response);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SupplierDto> Get(long id)
        {
            try 
            {
                var supplier = _service.GetSupplierById(id);
                if (supplier == null)
                    return NotFound();

                var result = _mapper.Map<Supplier, SupplierDto>(supplier);
                return Ok(result);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SupplierDto> Create([FromBody] NewSupplierDto data)
        {
            try
            {
                var supplier = _mapper.Map<NewSupplierDto, Supplier>(data);
                var newSupplier = _service.CreateSupplier(supplier);

                var result = _mapper.Map<Supplier, SupplierDto>(newSupplier);
                return CreatedAtAction(nameof(Get), new { id = newSupplier.Id }, result);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Update([FromBody] SupplierDto data)
        {
            try
            {
                var supplier = _mapper.Map<SupplierDto, Supplier>(data);
                _service.UpdateSupplier(supplier);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Delete(long id)
        {
            try
            {
                _service.DeleteSupplier(id);
                return NoContent();
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}