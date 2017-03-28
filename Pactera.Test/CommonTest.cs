using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pactera.Common.Npoi;
using Pactera.Data;
using Pactera.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Pactera.Test
{
    [TestClass]
    public class CommonTest
    {
        [TestMethod]
        public void TestDateTime()
        {
            // 这是一个时间是吧？在这个基础上减去一小时会是什么？测一下就知道了我再试试行吗，还想测试一下
            // 2015-07-22 14:00:00

            var caseCreateDate = DateTime.Now;
            var caseArriveDate = Convert.ToDateTime("2015-07-22 15:13:55.000");
            var caseYuzhi = caseCreateDate.AddHours(0 - 1);
            Console.WriteLine(caseCreateDate.AddHours(0 - 1).ToString("yyyy-MM-dd HH:mm:ss"));

            // 如果要是减去15个小时呢？会是什么？
            Console.WriteLine(caseCreateDate.AddHours(0 - 15).ToString("yyyy-MM-dd HH:mm:ss"));
            if (caseYuzhi > caseArriveDate) { Console.WriteLine("张春雨傻帽"); }
            // 看到了吗？你只需要减掉你想减去的时间就行，不用管它搜怎么计算的哦，知道了，那如果是当前时间我直接varcaseCreateDate = new DateTime();这样

        }
    }
}
