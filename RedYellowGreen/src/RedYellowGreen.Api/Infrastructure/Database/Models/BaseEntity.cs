using System.ComponentModel.DataAnnotations.Schema;

namespace RedYellowGreen.Api.Infrastructure.Database.Models;

public abstract class BaseEntity
{
    // using client generated ids, so no need to do a round trip to the database
    // to know what the id will be
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public DateTime CreatedAt { get; set; }
}