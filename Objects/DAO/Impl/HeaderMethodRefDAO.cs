using System;
using ChainLinkUtils.Base.DAO;
using NotificationsService.Objects.DTO;
using NotificationsService.Objects.DAO;
using System.Data.SqlClient;
using ChainLinkUtils.Utils;
using System.Data;
using Serilog;
using System.Collections.Generic;

namespace NotificationsService.Objects.DAO.Impl
{
    public class HeaderMethodRefDAO : AbstractBaseDAO, IHeaderMethodRefDAO
    {
        public HeaderMethodRefDAO()
        {
        }

        public int CreateHeaderMethodRef(HeaderMethodRefDTO refToCreate)
        {
            return CreateHeaderMethodRef(refToCreate.HeaderId, refToCreate.MethodId);
        }

        public int CreateHeaderMethodRef(int? headerId, int? methodId)
        {
            int id = -1;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_CreateHeaderMethodref", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@headerId", headerId);
                    command.Parameters.AddWithValue("@methodId", methodId);

                    id = Convert.ToInt32(command.ExecuteScalar());

                    if (id < 0)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (InvalidOperationException rbEx)
                        {
                            Log.Error("Exception rolling back transaction: " + rbEx.Message);
                            Log.Error(rbEx.StackTrace);
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception creating notification entry: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return id;
        }

        public bool DeleteHeaderMethodRef(HeaderMethodRefDTO refToDelete)
        {
            bool success = false;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = new SqlCommand("WebApplications.Notifications.sp_DeleteHeaderMethodRef", connection))
                using (SqlTransaction transaction = connection.BeginTransaction(Guid.NewGuid().ToString().Substring(0, 30)))
                {

                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@id", refToDelete.Id);

                    success = Convert.ToInt32(command.ExecuteNonQuery()) == 1;

                    if (!success)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (InvalidOperationException rbEx)
                        {
                            Log.Error("Exception rolling back transaction: " + rbEx.Message);
                            Log.Error(rbEx.StackTrace);
                        }
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception deleting header/method reference with Id [" + refToDelete.Id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return success;
        }

        public HeaderMethodRefDTO GetHeaderMethodRef(int? id)
        {
            HeaderMethodRefDTO refer = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  HeaderId, ";
                    sqlString += "  MethodId, ";
                    sqlString += "FROM WebApplications.Notifications.HeaderMethodRef ";
                    sqlString += "WHERE ";
                    sqlString += "  Id = @id ";

                    command.Parameters.AddWithValue("@id", id);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (refer == null)
                            {
                                refer = new HeaderMethodRefDTO()
                                {
                                    Id = Convert.ToInt32(dataReader["Id"]),
                                    HeaderId = Convert.ToInt32(dataReader["HeaderId"]),
                                    MethodId = Convert.ToInt32(dataReader["MethodId"])

                                };
                            }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving header/method reference by Id [" + id + "]: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return refer;
        }

        public HeaderMethodRefDTO GetHeaderMethodRef(int? headerId, int? methodId)
        {
            HeaderMethodRefDTO refer = null;

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  HeaderId, ";
                    sqlString += "  MethodId, ";
                    sqlString += "FROM WebApplications.Notifications.HeaderMethodRef ";
                    sqlString += "WHERE ";
                    sqlString += "  HeaderId = @headerId AND MethodId = @methodId";

                    command.Parameters.AddWithValue("@headerId", headerId);
                    command.Parameters.AddWithValue("@methodId", methodId);

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (refer == null)
                            {
                                refer = new HeaderMethodRefDTO()
                                {
                                    Id = Convert.ToInt32(dataReader["Id"]),
                                    HeaderId = Convert.ToInt32(dataReader["HeaderId"]),
                                    MethodId = Convert.ToInt32(dataReader["MethodId"])

                                };
                            }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving header/method reference for headerId [" + headerId + "], methodId [" + methodId + "] : " + e.Message);
                Log.Error(e.StackTrace);
            }

            return refer;
        }

        public List<HeaderMethodRefDTO> GetAllHeaderMethodRefs()
        {
            List<HeaderMethodRefDTO> referenceList = new List<HeaderMethodRefDTO>();

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  HeaderId, ";
                    sqlString += "  MethodId ";
                    sqlString += "FROM WebApplications.system.HeaderMethodRef ";

                    command.CommandText = sqlString;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            HeaderMethodRefDTO refer = new HeaderMethodRefDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                HeaderId = Convert.ToInt32(dataReader["HeaderId"]),
                                MethodId = Convert.ToInt32(dataReader["MethodId"])
                            };
                            referenceList.Add(refer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return referenceList;
        }

        public List<HeaderMethodRefDTO> GetAllHeaderMethodRefs(int? headerId)
        {
            List<HeaderMethodRefDTO> referenceList = new List<HeaderMethodRefDTO>();

            if (headerId == null)
            {
                return GetAllHeaderMethodRefs();
            }

            try
            {
                using (SqlConnection connection = SqlUtils.GetConnection(_config))
                using (SqlCommand command = connection.CreateCommand())
                {
                    string sqlString = string.Empty;
                    sqlString += "SELECT ";
                    sqlString += "  Id, ";
                    sqlString += "  HeaderId, ";
                    sqlString += "  MethodId ";
                    sqlString += "FROM WebApplications.system.HeaderMethodRef ";
                    sqlString += " WHERE HeaderId = @headerId ";

                    command.CommandText = sqlString;
                    command.Parameters.AddWithValue("@headerId", headerId);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            HeaderMethodRefDTO refer = new HeaderMethodRefDTO()
                            {
                                Id = Convert.ToInt32(dataReader["Id"]),
                                HeaderId = Convert.ToInt32(dataReader["HeaderId"]),
                                MethodId = Convert.ToInt32(dataReader["MethodId"])
                            };
                            referenceList.Add(refer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception retrieving list of notifications: " + e.Message);
                Log.Error(e.StackTrace);
            }

            return referenceList;
        }
    }
}
