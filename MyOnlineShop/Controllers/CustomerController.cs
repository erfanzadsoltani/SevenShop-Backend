using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using System.Security.Claims;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using MyOnlineShop.Models.apimodel;
using MyOnlineShop.Models;
using MyOnlineShop.Services;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using System.Data;

namespace MyOnlineShop.Controllers
{
    public class CustomerController : ControllerBase
    {
        private readonly MyShopContext _context;


        public CustomerController(MyShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("customers")]
        public ActionResult<IEnumerable<Pagination<customerModel>>> CustomersGet([FromQuery] int page, [FromQuery] int customersPerPage)
        {
            try
            {
                var length = _context.customer.ToList().Count();
                var totalPages = (int)Math.Ceiling((decimal)length / (decimal)customersPerPage);
                page = Math.Min(totalPages, page);
                var start = Math.Max((page - 1) * customersPerPage, 0);
                var end = Math.Min(page * customersPerPage, length);
                var count = Math.Max(end - start, 0);
                customersPerPage = count;
                var customers = new Pagination<customerModel>
                {
                    page = page,
                    totalPages = totalPages,
                    perPage = customersPerPage,
                    data = _context.customer
                            .Skip(start)
                            .Take(count).Select(u => new customerModel
                            {
                                id = u.UserId,
                                username = u.user.UserName,
                                firstName = u.user.FirstName,
                                lastName = u.user.LastName,
                                phoneNumber = u.user.PhoneNumber,
                                email = u.user.Email,
                                profileImage = u.user.ImageUrl,
                                birthDate = u.user.BirthDate,
                                restricted = u.user.Restricted,
                                address = u.Address,
                                balance = u.Balance
                            }).ToList()
                };
                if (customers == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(customers);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




        [HttpGet]
        [Route("customers/{id}")]
        public ActionResult<IEnumerable<customerModel>> EachCustomer(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else if (!_context.customer.Any(c => c.UserId == id))
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    Customer cc = _context.customer.Single(c=>c.UserId == id);
                    User user = _context.users.Single(f => f.ID == id);

                    customerModel schema = new customerModel()
                    {
                        id = cc.UserId,
                        username = user.UserName,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        phoneNumber = user.PhoneNumber,
                        email = user.Email,
                        profileImage = user.ImageUrl,
                        birthDate = user.BirthDate,
                        restricted = user.Restricted,
                        address = cc.Address,
                        balance = cc.Balance
                    };
                    return Ok(schema);
                }


            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPut]
        [Route("customers/{id}")]
        public ActionResult<IEnumerable<customerModel>> EachCustomerPut(Guid id, [FromBody] customerreqModel custupdate)
        {
            try
            {
                //var c1 = _context.customer.ToList();
                //Customer cc = null;
                //int i = 0;
                //foreach (var t in c1)
                //{
                //    if (t.ID == id)
                //    {
                //        cc = c1.Single(x => x.ID == id);
                //        i++;
                //    }
                //}

                //if (!_context.customer.Any(c => c.UserId == id)) {
                //    return StatusCode(StatusCodes.Status404NotFound);
                //}

                if (!_context.customer.Any(c => c.UserId == id))
                {
                    Logger.LoggerFunc($"Customers/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            custupdate, StatusCode(StatusCodes.Status404NotFound));
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else {
                    Customer cc = _context.customer.Single(c => c.UserId == id);
                    if (custupdate.address != null) cc.Address = custupdate.address;
                    cc.Balance += custupdate.balance;

                    _context.customer.UpdateRange();
                    _context.SaveChanges();
                    var user = _context.users.SingleOrDefault(f => f.ID == cc.UserId);
                    customerModel schema = new customerModel()
                    {
                        id = cc.UserId,
                        username = user.UserName,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        phoneNumber = user.PhoneNumber,
                        email = user.Email,
                        profileImage = user.ImageUrl,
                        birthDate = user.BirthDate,
                        restricted = user.Restricted,
                        address = cc.Address,
                        balance = cc.Balance
                    };
                    Logger.LoggerFunc($"Customers/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            custupdate, schema);
                    return Ok(schema);

                }
            }
            catch
            {
                Logger.LoggerFunc($"Customers/{id}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                            custupdate, StatusCode(StatusCodes.Status500InternalServerError));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
