using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using WIMI.BTL.BasicData;

namespace WIMI.BTL.BasicData.Dtos
{
    [AutoMap(typeof(QesUploadRecord))]
    public class QesUploadRecordDto: FullAuditedEntityDto<long>
    {
        public string PartCode { get; set; }

        public int Status { get; set; }

        public int Type { get; set; }

        public string SendMessage { get; set; }

        public string ReceiveMessage { get; set; }

        public string BackupPath { get; set; }

        public string Remark { get; set; }

    }
}

