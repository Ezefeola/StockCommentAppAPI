using Microsoft.EntityFrameworkCore;
using StockCommentApp.Data;
using StockCommentApp.Interfaces;
using StockCommentApp.Models;

namespace StockCommentApp.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _dbContext.Comments.AddAsync(commentModel);
            await _dbContext.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await 
                _dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id); 

            if (commentModel is null)
            {
                return null;
            }

            _dbContext.Comments.Remove(commentModel);
            await _dbContext.SaveChangesAsync();

            return commentModel;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _dbContext.Comments.Include(a => a.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var comment = await _dbContext.Comments
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id);


            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await _dbContext.Comments.FindAsync(id);

            if(existingComment is null)
            {
                return null;
            }

            existingComment.Id = id;

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _dbContext.SaveChangesAsync();

            return existingComment;
        }
    }
}
