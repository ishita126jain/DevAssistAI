using DevAssistAI.Data;
using DevAssistAI.Entites;
using DevAssistAI.Repository.Contract;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;

namespace DevAssistAI.Repository.Operation
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;
        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateSession(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task AddMessage(ChatMessages message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatMessages>> GetSessionMessage(Guid sessionId)
        {
            return await _context.ChatMessages.Where(m => m.ChatSessionId == sessionId).OrderBy(m => m.CreatedAt).ToListAsync();
        }

        public async Task<List<ChatSession>> GetSessions() 
        { 
            return await _context.ChatSessions.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }
        public async Task<List<ChatMessages>> GetMessages(Guid sessionId)
        {
            return await _context.ChatMessages.Where(x => x.ChatSessionId == sessionId).OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task<bool> SessionExists(Guid sessionId)
        {
            return await _context.ChatSessions.AnyAsync(s => s.Id == sessionId);
        }
    }
}
