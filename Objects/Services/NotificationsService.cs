using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NotificationsService.Objects.DAO;
using NotificationsService.Objects.DTO;
using ChainLinkUtils.Utils.Objects.DTO;
using ChainLinkUtils.Utils.Objects.Models;
using ChainLinkUtils.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text.Json;
using Serilog;
using UserAuthentication.Objects.Responses;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;

namespace NotificationsService.Objects.Services
{
    public class NotificationService
    {
        protected INotificationDAO _notificationDAO;
        protected INotificationRecipientDAO _notificationRecipientDAO;
        protected INotificationDetailsDAO _notificationDetailsDAO;
        protected INotificationMethodDAO _notificationMethodDAO;
        protected INotificationHistoryDAO _notificationHistoryDAO;
        protected IHeaderMethodRefDAO _headerMethodRefDAO;
        protected INotificationStoredProcedureDAO _notificationStoredProcedureDAO;

        public NotificationService(INotificationDAO notificationDAO, INotificationRecipientDAO notificationRecipientDAO,
            INotificationDetailsDAO notificationDetailsDAO, INotificationMethodDAO notificationMethodDAO,
            INotificationHistoryDAO notificationHistoryDAO, IHeaderMethodRefDAO headerMethodRefDAO,
            INotificationStoredProcedureDAO notificationStoredProcedureDAO)
        {
            _notificationDAO = notificationDAO;
            _notificationRecipientDAO = notificationRecipientDAO;
            _notificationDetailsDAO = notificationDetailsDAO;
            _notificationMethodDAO = notificationMethodDAO;
            _notificationHistoryDAO = notificationHistoryDAO;
            _headerMethodRefDAO = headerMethodRefDAO;
            _notificationStoredProcedureDAO = notificationStoredProcedureDAO;
        }

        /// <summary>
        /// Retrieves a list of all Notifications. If an <paramref name="applicationId"/> is provided,
        /// the list of items is filtered to the <paramref name="applicationId"/> provided.
        /// </summary>
        /// <returns>List of NotificationDTO objects. Returns empty list of dbo.Notifications is empty, 
        /// or if there are no entries for the <paramref name="applicationId"/>.
        /// 
        /// This throws a KeyNotFoundException if the <paramref name="applicationId"/> is not valid.</returns>
        /// <param name="applicationId">ApplicationId to search. This is an optional.</param>
        public List<NotificationDTO> GetAllNotifications()
        {

            return _notificationDAO.GetAllNotifications();
        }

        public List<NotificationHistoryDTO> GetAllNotificationHistory(int? appId, int? userId)
        {

            return _notificationHistoryDAO.GetAllNotificationHistory(appId, userId);
        }

        /// <summary>
        /// Retrieves the locale for the given <paramref name="id"/>
        /// </summary>
        /// <returns>NotificationDTO tied to the given <paramref name="id"/>. Throws 
        /// a KeyNotFoundException if no dbo.Notifications entry exists by the given <paramref name="id"/></returns>
        /// <param name="id">Id to search.</param>
        public NotificationDTO GetNotification(int? id)
        {
            NotificationDTO notification = _notificationDAO.GetNotification(id);
            if (notification == null)
            {
                throw new KeyNotFoundException();
            }

            notification.Messages = _notificationDetailsDAO.GetNotificationDetailsByNotificationId(id);
            notification.Methods = _notificationMethodDAO.GetNotificationMethodsByNotificationId(id);
            return notification;
        }

        /// <summary>
        /// Retrieves a given notification stored procedure <paramref name="id"/>
        /// </summary>
        /// <returns>NotificationStoredProcedureDTO tied to the given <paramref name="id"/>. Throws 
        /// a KeyNotFoundException if no dbo.Notification Stored Procedure entry exists by the given <paramref name="id"/></returns>
        /// <param name="id">Id to search.</param>
        public NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? id)
        {
            NotificationStoredProcedureDTO storedProc = _notificationStoredProcedureDAO.GetNotificationStoredProcedure(id);
            if (storedProc == null)
            {
                throw new KeyNotFoundException();
            }
            return storedProc;
        }

        public NotificationStoredProcedureDTO GetNotificationStoredProcedure(int? notificationId, string storedProcedure)
        {
            NotificationStoredProcedureDTO storedProc = _notificationStoredProcedureDAO.GetNotificationStoredProcedure(notificationId, storedProcedure);
            if (storedProc == null)
            {
                throw new KeyNotFoundException();
            }
            return storedProc;
        }

        public List<NotificationStoredProcedureDTO> GetNotificationStoredProcedures(int? notificationId)
        {
            List<NotificationStoredProcedureDTO> storedProcs = _notificationStoredProcedureDAO.GetNotificationStoredProcedures(notificationId);
            if (storedProcs == null)
            {
                throw new KeyNotFoundException();
            }
            return storedProcs;
        }

        public List<NotificationStoredProcedureDTO> GetNotificationsForStoredProcedure(string storedProcedure)
        {
            List<NotificationStoredProcedureDTO> storedProcs = _notificationStoredProcedureDAO.GetNotificationsForStoredProcedure(storedProcedure);
            if (storedProcs == null)
            {
                throw new KeyNotFoundException();
            }
            return storedProcs;
        }

        /// <summary>
        /// Creates an entry in the dbo.Notifications table.
        /// </summary>
        /// <returns>The Id of the newly created dbo.Notifications entry. Throws an ArgumentException if the
        /// unique key of dbo.Applications.Id and Name already exists.</returns>
        /// <param name="notificationToCreate">NotificationDTO object to create.</param>
        public int CreateNotification(NotificationDTO notificationToCreate)
        {
            if (string.IsNullOrEmpty(notificationToCreate.Description))
            {
                throw new MissingFieldException();
            }

            NotificationDTO existingEntry = _notificationDAO.GetNotification(notificationToCreate.Id);
            if (existingEntry != null)
            {
                throw new ArgumentException();
            }

            int newId = _notificationDAO.CreateNotification(notificationToCreate.Description);

            foreach (NotificationDetailDTO dtl in notificationToCreate.Messages)
            {
                dtl.NotificationId = newId;
                _notificationDetailsDAO.CreateNotificationDetails(dtl);
            }
            foreach (NotificationMethodDTO method in notificationToCreate.Methods)
            {
                HeaderMethodRefDTO refer = new HeaderMethodRefDTO();
                refer.HeaderId = newId;
                _headerMethodRefDAO.CreateHeaderMethodRef(refer);
            }

            return newId;
        }

        /// <summary>
        /// Creates an entry in the dbo.NotificationStoredProcedures table.
        /// </summary>
        /// <returns>The Id of the newly created dbo.NotificationStoredProcedures entry. Throws an ArgumentException if the
        /// unique key of dbo.Applications.Id and Name already exists.</returns>
        /// <param name="storedProcToCreate">NotificationDTO object to create.</param>
        public int CreateNotificationStoredProcedure(NotificationStoredProcedureDTO storedProcToCreate)
        {
            if (string.IsNullOrEmpty(storedProcToCreate.StoredProcedureName) || storedProcToCreate.NotificationId == null)
            {
                throw new MissingFieldException();
            }

            NotificationDTO existingEntry = _notificationDAO.GetNotification(storedProcToCreate.Id);
            if (existingEntry != null)
            {
                throw new ArgumentException();
            }

            int newId = _notificationStoredProcedureDAO.CreateNotificationStoredProcedure(storedProcToCreate);

            return newId;
        }


        /// <summary>
        /// Updates the dbo.Notifications entry associated with the given <paramref name="id"/>
        /// with the values from the given <paramref name="notificationToUpdate"/>. An application,
        /// name and value are required. A MissingFieldException is thrown if any 
        /// are <see langword="null"/> postmor empty.
        /// </summary>
        /// <returns><c>true</c>, if notification was updated, <c>false</c> otherwise. Throws a
        /// KeyNotFoundException if the <paramref name="id"/> provided doesn't exist in dbo.Notifications</returns>
        /// <param name="id">Id to update.</param>
        /// <param name="notificationToUpdate">NotificationDTO object to use to update.</param>
        public bool UpdateNotification(int id, NotificationDTO notificationToUpdate)
        {
            NotificationDTO existingEntry = _notificationDAO.GetNotification(notificationToUpdate.Id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException();
            }

            if (notificationToUpdate.Id == null ||
                string.IsNullOrEmpty(notificationToUpdate.Description))
            {
                throw new MissingFieldException();
            }

            if (notificationToUpdate.Id.HasValue && notificationToUpdate.Id.Value != id)
            {
                throw new ArgumentException("Id mismatch.");
            }

            return _notificationDAO.UpdateNotification(notificationToUpdate);
        }

        public bool UpdateNotificationStoredProcedure(int id, NotificationStoredProcedureDTO storedProcToUpdate)
        {
            NotificationStoredProcedureDTO existingEntry = _notificationStoredProcedureDAO.GetNotificationStoredProcedure(storedProcToUpdate.Id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException();
            }

            if (storedProcToUpdate.Id == null || storedProcToUpdate.NotificationId == null ||
                string.IsNullOrEmpty(storedProcToUpdate.StoredProcedureName))
            {
                throw new MissingFieldException();
            }

            if (storedProcToUpdate.Id != id)
            {
                throw new ArgumentException("Id mismatch.");
            }

            return _notificationStoredProcedureDAO.UpdateNotificationStoredProcedure(storedProcToUpdate);
        }


        public bool UpdateNotificationHistory(int? id, NotificationHistoryDTO historyToUpdate)
        {
            NotificationHistoryDTO existingEntry = _notificationHistoryDAO.GetNotificationHistory(historyToUpdate.Id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException();
            }

            if (historyToUpdate.Id == null)
            {
                throw new MissingFieldException();
            }

            if (historyToUpdate.Id.HasValue && historyToUpdate.Id.Value != id)
            {
                throw new ArgumentException("Id mismatch.");
            }

            return _notificationHistoryDAO.UpdateNotificationHistory(historyToUpdate);
        }

        /// <summary>
        /// Deletes the dbo.Notifications entry associated with the given <paramref name="id"/>.
        /// 
        /// TODO:
        /// This should validate the Session-ID header of the request at some point,
        /// and verify the user associated with the Session-ID has access to delete Notifications
        /// 
        /// </summary>
        /// <returns><c>true</c>, if notification was deleted, <c>false</c> otherwise.</returns>
        /// <param name="id">Id of dbo.Notifications entry to delete.</param>
        public bool DeleteNotification(int? id)
        {
            NotificationDTO existingEntry = _notificationDAO.GetNotification(id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException();
            }

            return _notificationDAO.DeleteNotification(existingEntry);
        }

        public bool DeleteNotificationStoredProcedure(int? id)
        {
            NotificationStoredProcedureDTO existingEntry = _notificationStoredProcedureDAO.GetNotificationStoredProcedure(id);
            if (existingEntry == null)
            {
                throw new KeyNotFoundException();
            }

            return _notificationStoredProcedureDAO.DeleteNotificationStoredProcedure(existingEntry);
        }

        public bool SendNotification(int? id, Dictionary<string, object> parameters, List<int> recipientUserIds, List<int> applicationIds, DateTime expirationDate)
        {
            NotificationDTO notification = GetNotification(id);

            if (notification == null)
            {
                throw new KeyNotFoundException();
            }

            // Call a function to get recipients and applications based on the input parameters

            List<NotificationRecipientDTO> recipients = _notificationRecipientDAO.GetNotificationRecipients(recipientUserIds, applicationIds);

            foreach (NotificationDetailDTO message in notification.Messages)

            {
                int? emailNotificationId = _notificationRecipientDAO.Config.GetSection("Notifications").GetSection("EmailNotificationId").Get<Int32>();
                bool? individualNotifications = _notificationRecipientDAO.Config.GetSection("Notifications").GetSection("IndividualMessages").Get<bool>();
                string? fromAddress = _notificationRecipientDAO.Config.GetSection("Notifications").GetSection("FromAddress").Get<string>();
                string? fromName = _notificationRecipientDAO.Config.GetSection("Notifications").GetSection("FromName").Get<string>();

                // Perform any variable replacement as needed
                string messageBody = message.MessageTemplate;
                string subject = notification.Description;

                List<AttachmentFile> attachments = new List<AttachmentFile>();

                // Automatically insert the current date into
                // the replacement variables.
                var currDateField = parameters.GetValueOrDefault("CurrentDate");
                if (currDateField == "" || currDateField == null)
                {
                    var currentDate = DateTime.Today.ToString("MM-dd-yyyy");
                    parameters.Add("CurrentDate", currentDate);
                }

                // Search through the parameters to replace occurrences in
                // the message body
                foreach (var param in parameters)
                {
                    // Report ID Parameters contain a JSON string that includes the report ID and any required parameters.
                    if (param.Key.ToString() == "ReportId")
                    {
                        ReportRunDetails reportDetails = new ReportRunDetails();
                        reportDetails.ReportId = Convert.ToInt16(param.Value.ToString());

                        string fileName = "";
                        var dldFileParam = parameters.GetValueOrDefault("DownloadFileName");
                        if (dldFileParam != null)
                        {
                            fileName = dldFileParam.ToString();

                            // If the file name contains replacement variables search through the
                            // parameter list to add the variable.
                            if (fileName.IndexOf("{{") > -1)
                                {
                                    foreach (var p in parameters)
                                    {
                                        string replStr = "{{" + p.Key.ToString() + "}}";
                                        if (fileName.IndexOf(replStr) >= 0)
                                        {
                                            fileName = fileName.Replace(replStr, p.Value.ToString());
                                        }
                                    }

                                }
                            }
                            else
                            {
                                fileName = "Report.xlsx";
                            }
                            reportDetails.DownloadFileName = Path.GetTempPath() + fileName;

                            var reportParameters = parameters.GetValueOrDefault("Parameters");
                            if (reportParameters != null)
                            {
                                Dictionary<string, object> rptParms = (Dictionary<string, object>)reportParameters;
                                foreach (var rptParm in rptParms)
                                {
                                    ReportParameter par = new ReportParameter();
                                    par.Name = rptParm.Key.ToString();
                                    par.Value = rptParm.Value.ToString();
                                    reportDetails.Parameters.Add(par);
                                }
                            }

                            FileContentResult file = GetReportAttachment(reportDetails, _notificationDAO.Config).Result;

                            if (file != null)
                            {
                                if (file.FileDownloadName == string.Empty)
                                {
                                    file.FileDownloadName = "Report.xlsx";
                                }
                                System.IO.File.WriteAllBytes(file.FileDownloadName, file.FileContents);
                                AttachmentFile attachmentFile = new AttachmentFile();

                                attachmentFile.FileName = file.FileDownloadName;
                                attachmentFile.MediaType = file.ContentType;

                                attachments.Add(attachmentFile);
                            }
                        }
                        else
                        {
                            string replacementString = "{{" + param.Key.ToString() + "}}";
                            if (messageBody.IndexOf(replacementString) >= 0)
                            {
                                messageBody = messageBody.Replace(replacementString, param.Value.ToString());
                            }
                            if (messageBody.IndexOf(replacementString) >= 0)
                            {
                                messageBody = messageBody.Replace(replacementString, param.Value.ToString());
                            }

                            if (subject.IndexOf(replacementString) >= 0)
                            {
                                subject = subject.Replace(replacementString, param.Value.ToString());
                            }
                            if (subject.IndexOf(replacementString) >= 0)
                            {
                                subject = subject.Replace(replacementString, param.Value.ToString());
                            }


                    }
                }

                    // Next go through the notification methods
                    foreach (NotificationMethodDTO method in notification.Methods)
                    {
                        List<MailNameDTO> recips = new List<MailNameDTO>();
                        recips.Clear();

                        MailNameDTO sender = new MailNameDTO()
                        {
                            EmailAddress = fromAddress,
                            DisplayName = fromName
                        };

                        foreach (NotificationRecipientDTO recipient in recipients)
                        {
                            var sent = true;
                            if (method.Id == emailNotificationId)
                            {
                                MailNameDTO recip = new MailNameDTO()
                                {
                                    EmailAddress = recipient.EmailAddress,
                                    DisplayName = recipient.LastName.TrimEnd() + ", " + recipient.FirstName.TrimEnd()
                                };

                                if (individualNotifications == true)
                                {
                                    recips.Clear();
                                    recips.Add(recip);

                                    try
                                    {
                                        Utils.SendEmail(_notificationRecipientDAO.Config, sender, recips, subject, messageBody, message.MessageFormat, attachments);
                                    }
                                    catch (Exception ex)
                                    {
                                        sent = false;
                                    }
                                }
                                else
                                {
                                    var check = recips.Find(e => e.EmailAddress == recip.EmailAddress);
                                    if (check == null)
                                    {
                                        recips.Add(recip);
                                    }
                                }
                            }
                            NotificationHistoryDTO note = new NotificationHistoryDTO();

                            note.ApplicationId = recipient.ApplicationId;
                            note.RecipientUserId = recipient.UserId;
                            note.NotificationId = id;
                            note.MessageBody = messageBody;
                            note.NotificationMethodId = method.Id;
                            if (method.Id == emailNotificationId)
                            {
                                note.ReadFlag = sent;
                                note.ExpiredFlag = true;
                                note.ExpirationDateTime = new DateTime(9999, 12, 31);
                            }
                            else
                            {
                                note.ReadFlag = false;
                                note.ExpiredFlag = false;
                                note.ExpirationDateTime = expirationDate;
                            }
                            _notificationHistoryDAO.CreateNotificationHistory(note);

                        }

                        if (method.Id == emailNotificationId && individualNotifications == false && recips.Count > 0)
                        {
                            try
                            {
                                Utils.SendEmail(_notificationRecipientDAO.Config, sender, recips, subject, messageBody, message.MessageFormat, attachments);
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                        }

                    }
                }

                return true;

            }

            static async Task<FileContentResult> GetReportAttachment(ReportRunDetails report, IConfiguration config)
            {
                string path = string.Empty;
                ApiLoginResponse login = null;

                // In this case we need to get a JWT token to pass to the report service.
                // to do that we will use an API key.
                string apiKey = config.GetSection("LoginAPIKey").Value;

                if (apiKey != string.Empty && apiKey != null)
                {
                    login = DoAPILogin(apiKey, config);
                }

                Log.Information("APIKey:" + apiKey);
                Log.Information("Token:" + login.Token);
                Log.Information("RefreshToken:" + login.RefreshToken.Token);
                Log.Information("RefreshTokenExpiration:" + login.RefreshToken.Expiration);

                path = config.GetSection("ExcelReportingUrl").Value;


                string basePath = new Uri(path).GetLeftPart(UriPartial.Authority);

                if (path != null && path != string.Empty)
                {
                    path = path.Replace("{{reportId}}", report.ReportId.ToString());
                }

                if (report.Parameters != null)
                {
                    bool first = true;
                    foreach (ReportParameter param in report.Parameters)
                    {
                        if (first)
                        {
                            path += "?";
                            first = false;
                        }
                        else
                        {
                            path += "&";
                        }

                        path += param.Name + "=" + param.Value;
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(basePath);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Add("TS-Refresh-Token", login.RefreshToken.Token);
                    client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };

                    FileContentResult file;
                    try
                    {
                        var send = client.GetByteArrayAsync(path);
                        var result = client.GetByteArrayAsync(path).Result;

                        file = new FileContentResult(result, "application/octet-stream")
                        {
                            FileDownloadName = report.DownloadFileName
                        };

                        return file;
                    }
                    catch (Exception ie)
                    {
                        //foreach (var ie in ae.InnerExceptions)
                        //{
                        Log.Error("Error:" + ie.Message);

                        Log.Error(ie.StackTrace.ToString());
                        //}
                        return null;
                    }
                }
            }

            private static ApiLoginResponse DoAPILogin(string apiKey, IConfiguration config)
            {

                using (HttpClient client = new HttpClient())
                {
                    string path = config.GetSection("UserAuthenticationUrl").Value;

                    path += "authenticate/api";

                    client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                    ApiLoginResponse login = null;
                    HttpResponseMessage response = client.GetAsync(path).Result;
                    var result = response.Content.ReadAsStringAsync();

                    string resultStr = result.Result;
                    login = System.Text.Json.JsonSerializer.Deserialize<ApiLoginResponse>(resultStr);
                    return login;
                }

            }
        }
    }