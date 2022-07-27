using System;
using System.Collections.Generic;
using ChainLinkUtils.Filters;
using ChainLinkUtils.Utils.Objects.Responses;
using NotificationsService.Objects.DAO;
using NotificationsService.Objects.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using NotificationsService.Objects.Services;
using NotificationsService.Objects.DAO.Impl;
using ChainLinkUtils.Utils.Objects.DTO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Notificationservice.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        protected IConfiguration _config;

        protected NotificationService _notificationsService;

        public NotificationsController(IConfiguration config, INotificationDAO notificationDAO, INotificationRecipientDAO notificationRecipientDAO,
            INotificationDetailsDAO notificationDetailsDAO, INotificationMethodDAO notificationMethodDAO, INotificationHistoryDAO notificationHistoryDAO,
            IHeaderMethodRefDAO headerMethodRefDAO, INotificationStoredProcedureDAO notificationStoredProcedureDAO)
        {
            _config = config;

            notificationDAO.Config = _config;
            notificationRecipientDAO.Config = _config;
            notificationMethodDAO.Config = _config;
            notificationDetailsDAO.Config = _config;
            notificationHistoryDAO.Config = _config;
            headerMethodRefDAO.Config = _config;
            notificationStoredProcedureDAO.Config = _config;

            _notificationsService = new NotificationService(notificationDAO, notificationRecipientDAO, notificationDetailsDAO, notificationMethodDAO, notificationHistoryDAO, headerMethodRefDAO, notificationStoredProcedureDAO);

        }

        // ****************************************
        // ***************** GET ******************
        // ****************************************

        [HttpGet("all")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult GetNotifications([FromQuery] int? applicationId = null)
        {
            try
            {
                List<NotificationDTO> allNotifications = _notificationsService.GetAllNotifications();

                return Ok(new { Notifications = allNotifications });
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in GetNotifications: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception retrieving Notifications: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult GetNotification(int id)
        {
            try
            {
                NotificationDTO notification = _notificationsService.GetNotification(id);

                return Ok(notification);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in GetNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception retrieving notification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }

        [HttpGet("history/{appId}/{userId}")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult GetNotificationHistory(int? appId, int? userId)
        {
            try
            {
                List<NotificationHistoryDTO> notificationHistory = _notificationsService.GetAllNotificationHistory(appId, userId);

                return Ok(notificationHistory);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in GetNotificationHistory: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception retrieving notification history: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }

        [HttpPut("history/{historyId}")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult UpdateNotificationHistory(int? historyId, [FromBody] NotificationHistoryDTO historyToUpdate)
        {
            try
            {
                _notificationsService.UpdateNotificationHistory(historyId, historyToUpdate);

                return Ok();
            }
            catch (ArgumentException e)
            {
                Log.Error("ArgumentException in UpdateNotificationHistory: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Bad Request."
                };

                return BadRequest(errorResponse);
            }
            catch (MissingFieldException e)
            {
                Log.Error("MissingFieldException in UpdateNotificationHistory: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Missing required fields in request."
                };

                return BadRequest(errorResponse);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in UpdateNotificationHistory: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception updating notification history: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }


        // ****************************************
        // ***************** POST *****************
        // ****************************************

        [HttpPost]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult CreateNotification([FromBody] NotificationDTO notificationToCreate)
        {
            try
            {
                int id = _notificationsService.CreateNotification(notificationToCreate);

                return Ok(new { Id = id });
            }
            catch (ArgumentException e)
            {
                Log.Error("ArgumentException in CreateNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Bad Request."
                };

                return BadRequest(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception creating notification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }

        // ****************************************
        // ***************** PUT ******************
        // ****************************************

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult UpdateNotification(int id, [FromBody] NotificationDTO notificationToUpdate)
        {
            try
            {
                _notificationsService.UpdateNotification(id, notificationToUpdate);

                return Ok();
            }
            catch (ArgumentException e)
            {
                Log.Error("ArgumentException in UpdateNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Bad Request."
                };

                return BadRequest(errorResponse);
            }
            catch (MissingFieldException e)
            {
                Log.Error("MissingFieldException in UpdateNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Missing required fields in request."
                };

                return BadRequest(errorResponse);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in UpdateNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception updating notification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }

        // ****************************************
        // ***************** DELETE ***************
        // ****************************************

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateJwtTokenFilter))]
        public ActionResult DeleteNotification(int id)
        {
            try
            {
                _notificationsService.DeleteNotification(id);

                return Ok();
            }
            catch (ArgumentException e)
            {
                Log.Error("ArgumentException in DeleteNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 400,
                    ErrorMessage = "Bad Request."
                };

                return BadRequest(errorResponse);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in DeleteNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception deleting notification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }


        // ****************************************
        // *********** SEND NOTIFICATION **********
        // ****************************************

        [HttpPost("send/{id}")]
        public ActionResult SendNotification(int id, [FromBody] NotificationRequestDTO notification)
        {
              // When called from a stored procedure bypass the JWTToken validation
            String fromStoredProc = Request.Headers["From-Stored-Proc"];

            Log.Debug("From Stored Procedure: " + fromStoredProc);
            if (fromStoredProc == null)
            {
                ValidateJwtTokenFilter validateJwt = new ValidateJwtTokenFilter(_config);
                if (!validateJwt.IsValidRequest(Request, Response))
                {
                    ErrorResponse errorResponse = new ErrorResponse()
                    {
                        ErrorCode = 403,
                        ErrorMessage = "Unauthorized Request."
                    };
                    return StatusCode(errorResponse.ErrorCode, errorResponse);
                }
            }
            else
            {
                try
                {
                    var spCheck = _notificationsService.GetNotificationStoredProcedure(id, fromStoredProc);

                    if (spCheck == null)
                    {
                        ErrorResponse errorResponse = new ErrorResponse()
                        {
                            ErrorCode = 403,
                            ErrorMessage = "Unauthorized Request."

                        };
                        return StatusCode(errorResponse.ErrorCode, errorResponse);
                    }
                }
                catch (KeyNotFoundException ex)
                {
                    ErrorResponse errorResponse = new ErrorResponse()
                    {
                        ErrorCode = 403,
                        ErrorMessage = "Unauthorized Request."
                    };
                    return StatusCode(errorResponse.ErrorCode, errorResponse);

                }
            }

            try
            {           

                Dictionary<string, object> parameters = notification.Parameters;
                List<int> recipientUserIds = notification.RecipientIds;
                List<int> applicationIds = notification.ApplicationIds;
                DateTime expirationDate = notification.ExpirationDateTime;

                Boolean returnStatus = _notificationsService.SendNotification(id, parameters, recipientUserIds, applicationIds, expirationDate);
                return Ok(returnStatus);
            }
            catch (KeyNotFoundException e)
            {
                Log.Error("KeyNotFoundException in SendNotification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 404,
                    ErrorMessage = "Bad Request."
                };

                return NotFound(errorResponse);
            }
            catch (Exception e)
            {
                Log.Error("Unexpected exception sending notification: " + e.Message);
                Log.Error(e.StackTrace);

                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = 500,
                    ErrorMessage = "There was an unexpected server error. Please try again later."
                };

                return StatusCode(errorResponse.ErrorCode, errorResponse);
            }
        }
    }
}