using System;
using Microsoft.EntityFrameworkCore;
using RealWorldUnitTest.Web.DAL;

namespace RealWorldUnitTest.Web.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task Create(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void Delete(T entity)
    {
        _context.Entry(entity).State = EntityState.Deleted;
        _context.SaveChanges();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }
}
