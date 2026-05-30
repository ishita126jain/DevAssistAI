using DevAssistAI.Data;
using DevAssistAI.Model;
using DevAssistAI.Repository.Contract;
using Microsoft.EntityFrameworkCore;

namespace DevAssistAI.Repository.Operation
{
    public class DocumentChunkRepository : IDocumentChunkRepository
    {
        private readonly ApplicationDbContext _context;
        public DocumentChunkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetActiveFileHash(string fileName, CancellationToken cancellationToken)
        {
            return await _context.DocumentChunk.Where(x => x.FileName == fileName && x.IsActive).Select(x => x.FileHash).FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }

        public async Task AddDocumentChunk(List<DocumentChunk> chunk, CancellationToken cancellationToken)
        {
            await _context.DocumentChunk.AddRangeAsync(chunk, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateChunks(string fileName, CancellationToken cancellationToken)
        {
            List<DocumentChunk> chunks = await _context.DocumentChunk.Where(x => x.FileName == fileName && x.IsActive).ToListAsync(cancellationToken);

            if (chunks.Any())
            {
                foreach (DocumentChunk chunk in chunks)
                {
                    chunk.IsActive = false;
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<DocumentChunk>> GetActiveChunks(CancellationToken cancellationToken)
        {
            return await _context.DocumentChunk.Where(x => x.IsActive).ToListAsync(cancellationToken);
        }
    }
}
