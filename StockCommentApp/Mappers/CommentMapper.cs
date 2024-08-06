using StockCommentApp.DTOs.Comment;
using StockCommentApp.Models;

namespace StockCommentApp.Mappers
{
    public static class CommentMapper
    { 

        public static CommentDTO ToCommentDTO(this Comment comment)
        {
            return new CommentDTO
            {
                Id = comment.Id,
                Title = comment.Title,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                CreatedBy = comment.AppUser.UserName,
                StockId = comment.StockId,
            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentDTO createCommentDTO, int stockId)
        {
            return new Comment
            {
                Title = createCommentDTO.Title,
                Content = createCommentDTO.Content,
                StockId = stockId
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentDTO updateCommentDTO)
        {
            return new Comment
            {
                Title = updateCommentDTO.Title,
                Content = updateCommentDTO.Content,
            };
        }
    }
}
