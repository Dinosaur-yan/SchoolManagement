﻿using SchoolManagement.Models;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Application.Dtos
{
    public class PaginationModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 每页分页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public List<Student> Data { get; set; }
    }
}
