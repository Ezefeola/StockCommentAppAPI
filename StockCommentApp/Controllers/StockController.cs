using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockCommentApp.Data;
using StockCommentApp.DTOs.Stock;
using StockCommentApp.Helpers;
using StockCommentApp.Interfaces;
using StockCommentApp.Mappers;
using StockCommentApp.Models;

namespace StockCommentApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IStockRepository _stockRepository;

        public StockController(ApplicationDbContext dbContext, IStockRepository stockRepository)
        {
            _dbContext = dbContext;
            _stockRepository = stockRepository;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stocks = await _stockRepository.GetAllAsync(query);

            var stockDto = stocks.Select(s => s.ToStockDTO()).ToList();

            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepository.GetByIdAsync(id);

            if (stock is null)
            {
                return NotFound();
            }

            var stockDTO = stock.ToStockDTO();


            return Ok(stockDTO);
        }

        [HttpPost] 
        public async Task<IActionResult> Create(CreateStockDTO createStockDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = createStockDTO.ToStockFromCreateDTO();

            await _stockRepository.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDTO());
        }

        [HttpPut("{id:int}")] 
        public async Task<IActionResult> Update(int id, UpdateStockDTO updateStockDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepository.UpdateAsync(id, updateStockDTO);

            if (stockModel is null) return NotFound();

            return Ok(stockModel.ToStockDTO());

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepository.DeleteAsync(id);

            if (stockModel is null) return NotFound();

            return NoContent();
        }

    }
}
