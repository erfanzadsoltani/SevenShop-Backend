using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
	public class Customer
	{
		[Key]
		public Guid ID { get; set; }
		public Guid UserId { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public double Balance { get; set; }
		[ForeignKey("UserId")]
		public User user { get; set; }
	}
}
