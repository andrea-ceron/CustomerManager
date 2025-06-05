
using System.ComponentModel.DataAnnotations.Schema;
namespace CustomerManager.Repository.Model;


public class TransactionalOutbox
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	public string Tabella { get; set; } = string.Empty;
	public string Messaggio { get; set; } = string.Empty;
}
