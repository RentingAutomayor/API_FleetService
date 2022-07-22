using System;
using System.Linq;
using System.Web.Http;
using API_FleetService.ViewModels;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class AuthController : ApiController {
        [HttpPost]
        public IHttpActionResult Save(UserViewModel userViewModel)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
                    var isRepeat = db.Users.Where(user => user.email == userViewModel.email).Count();
                    if (isRepeat > 0)
                    {
                        string usernameRepeat = "Ya existe un usuario con ese correo electronico. Intente con otro";
                        return BadRequest(new ResponseApiViewModel().Message = usernameRepeat);
                    }
                    Users users = AuthController.dtoToDomain(userViewModel);
                    db.Users.Add(users);
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult Update(UserViewModel userViewModel)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var userBd = db.Users.Where(u => u.usr_id == userViewModel.id).FirstOrDefault();
                    if (userBd is null) {
                        string notExists = "El usuario que desea editar no existe.";
                        return BadRequest(new ResponseApiViewModel().Message = notExists);
                    }
                    userBd.usr_firstName = userViewModel.name;
                    userBd.usr_lastName = userViewModel.lastName;
                    userBd.cpn_id = userViewModel.company?.id;
                    userBd.grp_id = userViewModel.roleId;
                    userBd.cli_id = userViewModel.clientId;
                    userBd.deal_id = userViewModel.dealerId;
                    userBd.user_status = userViewModel.status;
                    db.SaveChanges();
                    return Ok();
                }
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            try {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities()) {
                    var users = db.Users.Select(user => new UserViewModel() {
                        id = user.usr_id,
                        name = user.usr_firstName,
                        lastName = user.usr_lastName,
                        email = user.email,
                        roleId = user.grp_id,
                        company = new CompanyViewModel()
                        {
                            id = user.Company.cpn_id == null ? 0 : user.Company.cpn_id,
                            name = user.Company.cpn_razonSocial == null ? "" : user.Company.cpn_razonSocial,
                            nit = user.Company.cpn_nit == null ? "" : user.Company.cpn_nit
                        },
                        status = user.user_status
                    }).ToList();
                    return Ok(users);
                }
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var user = db.Users.Where(u => u.usr_id == id)
                        .Select(u => new UserViewModel() {
                            id = u.usr_id,
                            name = u.usr_firstName,
                            lastName = u.usr_lastName,
                            email = u.email,
                            roleId = u.grp_id,
                            password = u.usr_password,
                            company = new CompanyViewModel()
                            {
                                id = u.Company.cpn_id == null ? 0 : u.Company.cpn_id,
                                name = u.Company.cpn_razonSocial == null ? "" : u.Company.cpn_razonSocial,
                                nit = u.Company.cpn_nit == null ? "" : u.Company.cpn_nit
                            },
                            status = u.user_status,
                            clientId = u.cli_id,
                            dealerId = u.deal_id
                        }).FirstOrDefault();
                    if (user is null)
                    {
                        string notFound = "El usuario que desea buscar no se encuentra.";
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    }
                    return Ok(user);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteById(int id)
        {
            try
            {
                using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
                {
                    var user = db.Users.Find(id);
                    if (user is null)
                    {
                        string notFound = "El usuario que desea eliminar no existe.";
                        return BadRequest(new ResponseApiViewModel().Message = notFound);
                    }
                    user.user_status = "I";
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private static Users dtoToDomain(UserViewModel userViewModel)
        {
            return new Users() {
                usr_id = userViewModel.id,
                usr_firstName = userViewModel.name,
                usr_lastName = userViewModel.lastName,
                usr_password = userViewModel.password,
                cpn_id = userViewModel.company?.id,
                email = userViewModel.email,
                grp_id = userViewModel.roleId,
                user_status = userViewModel.status,
                cli_id = userViewModel.clientId,
                deal_id = userViewModel.dealerId
            };
        }
    }
}