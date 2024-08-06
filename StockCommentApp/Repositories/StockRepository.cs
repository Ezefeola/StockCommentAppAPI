using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using StockCommentApp.Data;
using StockCommentApp.DTOs.Stock;
using StockCommentApp.Helpers;
using StockCommentApp.Interfaces;
using StockCommentApp.Models;

namespace StockCommentApp.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StockRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _dbContext.Stocks.AddAsync(stockModel);
            await _dbContext.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel is null)
            {
                return null;
            }

            _dbContext.Stocks.Remove(stockModel);
            await _dbContext.SaveChangesAsync();

            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _dbContext.Stocks
                .Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol)) {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending 
                        ? stocks.OrderByDescending(s => s.Symbol) 
                        : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _dbContext.Stocks
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public Task<bool> StockExists(int id)
        {
            return _dbContext.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockDTO updateStockDTO)
        {
            var stockToUpdate = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if(stockToUpdate is null)
            {
                return null;
            }

            stockToUpdate.Symbol = updateStockDTO.Symbol;
            stockToUpdate.CompanyName = updateStockDTO.CompanyName;
            stockToUpdate.Purchase = updateStockDTO.Purchase;
            stockToUpdate.LastDiv = updateStockDTO.LastDiv;
            stockToUpdate.Industry = updateStockDTO.Industry;
            stockToUpdate.MarketCap = updateStockDTO.MarketCap;

            await _dbContext.SaveChangesAsync();

            return stockToUpdate;
        }
    }
}
