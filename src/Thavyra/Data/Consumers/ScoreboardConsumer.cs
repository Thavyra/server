using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Thavyra.Contracts;
using Thavyra.Contracts.Scoreboard;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Consumers;

public class ScoreboardConsumer :
    IConsumer<Objective_Create>,
    IConsumer<Objective_Delete>,
    IConsumer<Objective_GetByApplication>,
    IConsumer<Objective_GetById>,
    IConsumer<Objective_Update>,
    IConsumer<Score_Create>,
    IConsumer<Score_Delete>,
    IConsumer<Score_GetById>,
    IConsumer<Score_GetByUser>
{
    private readonly ThavyraDbContext _dbContext;

    public ScoreboardConsumer(ThavyraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private Objective MapObjective(ObjectiveDto objective)
    {
        return new Objective
        {
            Id = objective.Id,
            ApplicationId = objective.ApplicationId,

            Name = objective.Name,
            Scores = objective.Scores?.Select(MapScore).ToList() ?? [],

            CreatedAt = objective.CreatedAt
        };
    }

    private Score MapScore(ScoreDto score)
    {
        return new Score
        {
            Id = score.Id,
            ObjectiveId = score.ObjectiveId,
            UserId = score.UserId,

            Value = score.Value,

            CreatedAt = score.CreatedAt
        };
    }
    
    public async Task Consume(ConsumeContext<Objective_Create> context)
    {
        var objective = new ObjectiveDto
        {
            ApplicationId = context.Message.ApplicationId,
            Name = context.Message.Name,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Objectives.Add(objective);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(MapObjective(objective));
    }

    public async Task Consume(ConsumeContext<Objective_Delete> context)
    {
        await _dbContext.Objectives
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);

        if (context.IsResponseAccepted<Success>()) 
            await context.RespondAsync(new Success());
    }

    public async Task Consume(ConsumeContext<Objective_GetByApplication> context)
    {
        var objectives = await _dbContext.Objectives
            .Where(x => x.ApplicationId == context.Message.ApplicationId)
            .Include(x => x.Scores)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Objective>(objectives.Select(MapObjective).ToList()));
    }

    public async Task Consume(ConsumeContext<Objective_GetById> context)
    {
        var objective = await _dbContext.Objectives
            .Include(x => x.Scores)
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (objective is null)
        {
            if (context.IsResponseAccepted<NotFound>())
                await context.RespondAsync(new NotFound());
            
            return;
        }

        await context.RespondAsync(MapObjective(objective));
    }

    public async Task Consume(ConsumeContext<Objective_Update> context)
    {
        var objective = await _dbContext.Objectives.FindAsync(context.Message.Id, context.CancellationToken);

        if (objective is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        objective.Name = context.Message.Name;
        
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(MapObjective(objective));
    }

    public async Task Consume(ConsumeContext<Score_Create> context)
    {
        var score = new ScoreDto
        {
            ObjectiveId = context.Message.ObjectiveId,
            UserId = context.Message.UserId,
            Value = context.Message.Value,
            CreatedAt = DateTime.UtcNow
        };
        
        _dbContext.Scores.Add(score);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(MapScore(score));
    }

    public async Task Consume(ConsumeContext<Score_Delete> context)
    {
        await _dbContext.Scores
            .Where(x => x.Id == context.Message.Id)
            .ExecuteDeleteAsync(context.CancellationToken);

        await context.RespondAsync(new Success());
    }
    
    public async Task Consume(ConsumeContext<Score_GetById> context)
    {
        var score = await _dbContext.Scores
            .FirstOrDefaultAsync(x => x.Id == context.Message.Id, context.CancellationToken);

        if (score is null)
        {
            await context.RespondAsync(new NotFound());
            return;
        }
        
        await context.RespondAsync(MapScore(score));
    }

    public async Task Consume(ConsumeContext<Score_GetByUser> context)
    {
        var scores = await _dbContext.Scores
            .Where(x => x.UserId == context.Message.UserId)
            .ToListAsync(context.CancellationToken);

        await context.RespondAsync(new Multiple<Score>(scores.Select(MapScore).ToList()));
    }
}