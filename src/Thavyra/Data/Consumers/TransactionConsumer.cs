using MassTransit;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
using Thavyra.Contracts.Transaction;
using Thavyra.Contracts.User;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class TransactionConsumer :
    IConsumer<Transaction_Create>,
    IConsumer<Transfer_Create>,
    IConsumer<Transaction_GetById>,
    IConsumer<Transaction_GetByApplication>,
    IConsumer<Transaction_GetByUser>
{
    private readonly ThavyraDbContext _dbContext;

    public TransactionConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private Transaction Map(TransactionDto transaction)
    {
        return new Transaction
        {
            Id = transaction.Id,
            ApplicationId = transaction.ApplicationId,
            SubjectId = transaction.SubjectId,
            RecipientId = transaction.RecipientId,

            IsTransfer = transaction.RecipientId.HasValue,
            Description = transaction.Description,
            Amount = transaction.Amount,

            CreatedAt = transaction.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<Transaction_Create> context)
    {
        var subject = await _dbContext.Users.FindAsync(context.Message.SubjectId, context.CancellationToken);

        if (subject is null)
        {
            throw new InvalidOperationException("Subject not found.");
        }
        
        if (subject.Balance + context.Message.Amount < 0)
        {
            await context.RespondAsync(new InsufficientBalance());
            return;
        }

        var transaction = new TransactionDto
        {
            ApplicationId = context.Message.ApplicationId,
            SubjectId = context.Message.SubjectId,
            Description = context.Message.Description,
            Amount = context.Message.Amount,
        };

        _dbContext.Transactions.Add(transaction);
        subject.Balance += transaction.Amount;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new Transaction
        {
            Id = transaction.Id,
            ApplicationId = transaction.ApplicationId,
            SubjectId = transaction.SubjectId,

            IsTransfer = false,
            Description = transaction.Description,
            Amount = transaction.Amount,

            CreatedAt = transaction.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<Transfer_Create> context)
    {
        var subject = await _dbContext.Users.FindAsync(context.Message.SubjectId, context.CancellationToken);
        var recipient = await _dbContext.Users.FindAsync(context.Message.RecipientId, context.CancellationToken);
        
        if (subject is null || recipient is null)
        {
            throw new InvalidOperationException("Subject or recipient not found.");
        }
        
        if (subject.Balance - context.Message.Amount < 0)
        {
            await context.RespondAsync(new InsufficientBalance());
            return;
        }

        var transfer = new TransactionDto
        {
            ApplicationId = context.Message.ApplicationId,
            SubjectId = context.Message.SubjectId,
            RecipientId = context.Message.RecipientId,
            Description = context.Message.Description,
            Amount = context.Message.Amount,
        };
        
        _dbContext.Transactions.Add(transfer);
        subject.Balance -= transfer.Amount;
        recipient.Balance += transfer.Amount;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new Transaction
        {
            Id = transfer.Id,
            ApplicationId = transfer.ApplicationId,
            SubjectId = transfer.SubjectId,
            RecipientId = transfer.RecipientId,

            IsTransfer = true,
            Description = transfer.Description,
            Amount = transfer.Amount,

            CreatedAt = transfer.CreatedAt
        });
    }

    public async Task Consume(ConsumeContext<Transaction_GetById> context)
    {
        var transaction = await _dbContext.Transactions
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (transaction is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }

        await context.RespondAsync(Map(transaction));
    }

    public async Task Consume(ConsumeContext<Transaction_GetByApplication> context)
    {
        var transactions = await _dbContext.Transactions
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Transaction>(transactions.Select(Map).ToList()));
    }

    public async Task Consume(ConsumeContext<Transaction_GetByUser> context)
    {
        var transactions = await _dbContext.Transactions
            .Where(x => x.SubjectId == context.Message.UserId || x.RecipientId == context.Message.UserId)
            .ToListAsync(context.CancellationToken);
        
        await context.RespondAsync(new Multiple<Transaction>(transactions.Select(Map).ToList()));
    }
}