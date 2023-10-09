using Service.Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Identity.Application.Common
{
    public class ConsumerListAccepted<TContract> where TContract : IContract
    {
        public List<TContract> Data { get; set; }
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }
    public class ConsumerListAccepted<TContract, TTotalSchema> where TContract : IContract where TTotalSchema : class
    {
        public List<TContract> Data { get; set; }
        public TTotalSchema? Total { get; set; }
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }

    public class ConsumerPaginatedListAccepted<TContract> where TContract : IContract
    {
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public IPagination Pagination { set; get; }
        public List<TContract> Data { set; get; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }

    public class ConsumerPaginatedListAccepted<TContract, TSchema> where TContract : IContract where TSchema : class
    {
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public IPagination Pagination { set; get; }
        public List<TContract> Data { set; get; }
        public TSchema? Total { get; set; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }

    public class ConsumerAccepted<TContract> where TContract : IContract
    {
        public TContract? Data { get; set; }
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }

    public class ConsumerReportAccepted<TContract> where TContract : IContract
    {
        public TContract? Data { set; get; }
        public Dictionary<string, string> MetaData { get; set; }
        public string ReportFile { get; set; }
        public int Version { get; set; }
        public ConsumerStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { set; get; }
        public List<string>? Warning { set; get; }
        public List<string>? Info { set; get; }
    }

    public class ConsumerRejected
    {
        public ConsumerStatusCode StatusCode { get; set; }
        public string Reason { set; get; }
        public List<string>? Errors { set; get; }
    }
}
