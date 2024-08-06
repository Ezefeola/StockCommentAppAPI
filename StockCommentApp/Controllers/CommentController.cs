using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockCommentApp.DTOs.Comment;
using StockCommentApp.Extensions;
using StockCommentApp.Interfaces;
using StockCommentApp.Mappers;
using StockCommentApp.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StockCommentApp.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICommentRepository commentRepository, 
            IStockRepository stockRepository, UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comments = await _commentRepository.GetAllAsync();

            var commentDTO = comments.Select(s => s.ToCommentDTO());

            return Ok(commentDTO);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepository.GetByIdAsync(id);    

            if (comment is null)
            {
                return NotFound();
            }

            return Ok(comment.ToCommentDTO());
        }


        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create(int stockId, 
            CreateCommentDTO commentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _stockRepository.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            var username = User.GetUsername();

            var appUser = await _userManager.FindByNameAsync(username);
            var commentModel = commentDTO.ToCommentFromCreate(stockId);
            commentModel.AppUserId = appUser.Id;

            await _commentRepository.CreateAsync(commentModel);

            return Ok(commentModel);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateCommentDTO updateCommentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentRepository
                .UpdateAsync(id, updateCommentDTO.ToCommentFromUpdate());

            if(comment is null)
            {
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDTO());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commentModel = await _commentRepository.DeleteAsync(id);

            if (commentModel is null)
            {
                return NotFound("Comment does not exist");
            }

            return Ok(commentModel);
        }
    }
}
