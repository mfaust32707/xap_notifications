using System;
using System.Collections.Generic;
using ChainLinkUtils.Base.DAO;
using Microsoft.Extensions.Configuration;
using NotificationsService.Objects.DTO;

namespace NotificationsService.Objects.DAO
{
    public interface IHeaderMethodRefDAO: IBaseDAO
    {
        HeaderMethodRefDTO GetHeaderMethodRef(int? referenceId);
        HeaderMethodRefDTO GetHeaderMethodRef(int? headerId, int? methodId);
        List<HeaderMethodRefDTO> GetAllHeaderMethodRefs();
        List<HeaderMethodRefDTO> GetAllHeaderMethodRefs(int? headerId);
        int CreateHeaderMethodRef(int? headerId, int? methodId);
        int CreateHeaderMethodRef(HeaderMethodRefDTO headerMethodRef);
        bool DeleteHeaderMethodRef(HeaderMethodRefDTO headerMethodRef);
       
    }
}
