using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebGrease.Css.Extensions;
using HiveServer.Models;
using HiveServer.Providers;
using HiveServer.Results;
using HiveServer.DTO;
using HiveServer.Base;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Base
{
    public static class Utils
    {
        /// <summary>
        /// Check validity of the data re3cieved from the client, and correctly formats the error reponce if it's not valid
        /// </summary>
        /// <param name="model">The data that was received from the clinet and must b validity checked</param>
        /// <param name="request">HTTPrequest that was recieved, this is used for correct serialisation using content negotiation that has occured</param>
        /// <param name="validateAllProperties">Don't override unless you have a good reason</param>
        /// <returns>Correctly formatted HTTPResponse mesasge with erros if erros have occured, or simply empty responce message with OK status code</returns>
        public static HttpResponseMessage CheckModel(IValidatableObject model, HttpRequestMessage request)
        {
            bool validateAllProperties = true;

            if (model == null)
            { return request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.NoDataRecieved); }


            ValidationContext validationContext = new ValidationContext(model, null, null); 
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties); 


            if (! isValid)
            {
                string validatioErrors = string.Empty;
                int i = 1; 

                foreach(var validationErrror in validationResults)
                {
                    validatioErrors += String.Format(" {0}:{1}", i++, validationErrror.ErrorMessage); 
                }                

                return request.CreateResponse(HttpStatusCode.BadRequest, ErrorResponse.CreateModelError(validatioErrors));
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}