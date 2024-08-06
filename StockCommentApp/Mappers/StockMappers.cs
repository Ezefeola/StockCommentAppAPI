using StockCommentApp.DTOs.Stock;
using StockCommentApp.Models;

namespace StockCommentApp.Mappers
{
    public static class StockMappers
    {
        public static StockDTO ToStockDTO(this Stock stockModel)
        {
            return new StockDTO
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c => c.ToCommentDTO()).ToList(),
            };
        }

        public static Stock ToStockFromCreateDTO(this CreateStockDTO createStockDTO)
        {
            return new Stock
            {
                Symbol = createStockDTO.Symbol,
                CompanyName = createStockDTO.CompanyName,
                Purchase = createStockDTO.Purchase,
                LastDiv = createStockDTO.LastDiv,
                Industry = createStockDTO.Industry,
                MarketCap = createStockDTO.MarketCap,
            };
        }
    }
}
