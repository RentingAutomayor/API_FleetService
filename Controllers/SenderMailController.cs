using API_FleetService.Models;
using API_FleetService.ViewModels;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Web.Http;
using DAO_FleetService;

namespace API_FleetService.Controllers
{
    public class SenderMailController : ApiController
    {
        // POST: 
        [HttpPost]
        public string send(SenderMail sendermail)
        {
            EmailSettings emailSettings = new EmailSettings();
            using (DB_FleetServiceEntities db = new DB_FleetServiceEntities())
            {
                emailSettings = db.EmailSettings.Find(1);
            }
                // CONFIGURACION
                var mailMessage = new MimeMessage();

            mailMessage.From.Add(new MailboxAddress(sendermail.nameMessage, "senior.developer.sinapsist@gmail.com"));

            foreach (string mailtos in sendermail.emailReceiver)
            {
                mailMessage.To.Add(new MailboxAddress("", mailtos));
            }

            mailMessage.Subject = subjectres(sendermail.typemessage,sendermail.nOrderwork);
  
            mailMessage.Body = htmlres(sendermail.typemessage, sendermail.nOrderwork, sendermail.nDealer).ToMessageBody(); 

            // CONEXION
            using (var smtpClient = new SmtpClient())

            {

                smtpClient.Connect("smtp.gmail.com", 465, true);

                smtpClient.Authenticate(emailSettings.email, emailSettings.password);

                smtpClient.Send(mailMessage);

                smtpClient.Disconnect(true);

            }

            return "Succes";
        }

        public string subjectres(int typemessage, string nOrderwork)
        {
            string textresponse = "";

            switch (typemessage)
            {
                // Mensaje enviado al cliente luego de que el consesionario cree la orden de trabajo
                case (int)EnumMovement.ORDEN_DE_TRABAJO:
                    textresponse = string.Format(@"Orden de trabajo creada para aprobar - No {0}", nOrderwork);
                    break;
                // mensaje enviado al consesionario luego de que el cliente apruebe la ooden de trabajo
                case (int)EnumMovement.APROBACION_ORDEN_DE_TRABAJO:
                    textresponse = string.Format(@"Orden de trabajo aprobada - No {0}", nOrderwork);
                    break;
                // mensaje enviado al consesionario luego de que el cliente rechaze la orden de trabajo
                case (int)EnumMovement.CANCELACION_ORDEN_DE_TRABAJO:
                    textresponse = string.Format(@"Orden de trabajo rechazada - No {0}", nOrderwork);
                    break;
                // mensaje enviado al cliente luego de que el consesionario finalize la orden de trabajo
                case (int)EnumMovement.FINALIZACION_ORDEN_DE_TRABAJO:
                    textresponse = string.Format(@"Orden de trabajo ejecutada - No {0}", nOrderwork);
                    break;
                default:
                    throw new Exception("invalid option typemessage request bad");
            }
            
            
            return textresponse;

        }

        public BodyBuilder htmlres(int typemessage, string nOrderwork, string nDealer) {

            var bodyBuilder = new BodyBuilder();

            switch (typemessage)
            {   // Mensaje enviado al cliente luego de que el consesionario cree la orden de trabajo
                case (int)EnumMovement.ORDEN_DE_TRABAJO:
                    bodyBuilder.HtmlBody = string.Format(@"<p>Buen día,
                    <br>
                    <br>  
                    <br>
                    <br>
                    La orden de trabajo No <b>{0}</b> ha sido creada por el concesionario <b>{1}</b>, con el fin de que se revise y se apruebe.
        
                        En caso de tener alguna observación al respecto, por favor rechazar la orden de trabajo y poner los comentarios correspondientes.
                    </p>
                    <br>
                    <br>
                    <p>Ingrese aquí para revisar el detalle.</p>
                    <small>
                        <i>
                            <b>
                                <a href='#' >
                                    test link ####################### 
                                </a> 
                            </b>
                        </i>
                    </small>"
                    , nOrderwork, nDealer);
                break;
                // mensaje enviado al consesionario luego de que el cliente apruebe la ooden de trabajo
                case (int)EnumMovement.APROBACION_ORDEN_DE_TRABAJO:
                    bodyBuilder.HtmlBody = string.Format(@"<p>Buen día,
                    <br>
                    <br>  
                    <br>
                    <br>
                    La orden de trabajo No {0} fue aprobada por el cliente.
        
                    Dado lo anterior, se pueden iniciar con los procesos correspondientes.
                    </p>
                    <br>
                    <br>
                    <p>Ingrese aquí para revisar el detalle.</p>
                    <small>
                        <i>
                            <b>
                                <a href='#' >
                                    test link ####################### 
                                </a> 
                            </b>
                        </i>
                    </small>"
                    , nOrderwork);
                break;
                // mensaje enviado al consesionario luego de que el cliente rechaze la orden de trabajo
                case (int)EnumMovement.CANCELACION_ORDEN_DE_TRABAJO:
                    bodyBuilder.HtmlBody = string.Format(@"<p>Buen día,
                    <br>
                    <br>  
                    <br>
                    <br>
                    La orden de trabajo No {0} fue rechazada por el cliente.
        
                    Dado lo anterior, se pueden iniciar con los procesos correspondientes.
                    </p>
                    <br>
                    <br>
                    <p>Ingrese aquí para revisar el detalle.</p>
                    <small>
                        <i>
                            <b>
                                <a href='#' >
                                    test link ####################### 
                                </a> 
                            </b>
                        </i>
                    </small>"
                    , nOrderwork);
                break;
                // mensaje enviado al cliente luego de que el consesionario finalize la orden de trabajo
                case (int)EnumMovement.FINALIZACION_ORDEN_DE_TRABAJO:
                    bodyBuilder.HtmlBody = string.Format(@"<p>Buen día,
                            <br>
                            <br>  
                            <br>
                            <br>
                            La orden de trabajo No {0} aprobada por usted, ha sido finalizada lo cual indica que llevó a cabo el servicio.
        
                            En caso de tener alguna observación al respecto, por favor comunicarse directamente con el concesionario que realizó el mantenimiento.
                        </p>
                        <br>
                        <br>
                        <p>Ingrese aquí para revisar el detalle.</p>
                        <small>
                            <i>
                                <b>
                                    <a href='#' >
                                        test link ####################### 
                                    </a> 
                                </b>
                            </i>
                        </small>"
                    , nOrderwork);
                break;


                default:
                    throw new Exception("property selected error");
            
            }

            return bodyBuilder;
        }


    }
}
