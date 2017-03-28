using Pactera.Model;
using System.Linq;
using System.Collections;
using LitJson;
using Pactera.Data;

namespace Pactera.Core.Workflow
{
	partial class WorkflowEngine
    {
        #region >>发起工作流<<
        /// <summary>
        /// 发起工作流
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse Start(JsonData argsJsonData)
        {
            string argsJson = JsonMapper.ToJson(argsJsonData);

            var htField = new Hashtable();
            htField.Add("CaseId", argsJsonData["CaseId"].ToString());
            htField.Add("Creator", argsJsonData["Creator"].ToString());
            htField.Add("Engineer", argsJsonData["Engineer"].ToString());
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", argsJsonData["Engineer"].ToString());
            htField.Add("State", "Created");

            int workflowId = 0;
            var dbm = DataBaseFactory.Instance.Create();

            // 1. 创建工作流并显示ID
            dbm.Insert("Cus_Workflow", htField, out workflowId);
            if (workflowId < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowFail);

            /////获取工单的创建人//////
            var dtCreated = dbm.GetDataTable("*", "Cus_case", argsJsonData["CaseId"].ToString(), "Id");
            string Created = dtCreated.Rows[0]["Created"].ToString();
            var SysUserName = dbm.GetDataTable("Name", "Sys_User", Created, "Id");

            // 2. 创建日志
            htField.Clear();
            htField.Add("WorkflowId", workflowId);
            htField.Add("Action", WorkflowLogAction.Created.ToString());
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", argsJsonData["Engineer"].ToString());
            htField.Add("Message", SysUserName.Rows[0]["Name"].ToString()+"创建工单");

            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }

        /// <summary>
        /// 接收任务
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse AccetptTask(JsonData argsJsonData)
        {
            
            var dbm = DataBaseFactory.Instance.Create();
            //修改工作刘表
            Hashtable htField =  new Hashtable();
            string Id = argsJsonData["Id"].ToString();
            htField.Add("Creator", argsJsonData["Creator"].ToString());
            htField.Add("Engineer", argsJsonData["Engineer"].ToString());
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", argsJsonData["Receiver"].ToString());
            htField.Add("State", "Processing");
            htField.Add("CreateTime", argsJsonData["CreateTime"].ToString());
            int AccetptTask = dbm.Update("Cus_Workflow",htField,Id,"Id");

            ////////添加工作流日志////////
            htField.Clear();
            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", argsJsonData["Receiver"].ToString());
            htField.Add("Action", WorkflowLogAction.Handle.ToString());
            htField.Add("Message", argsJsonData["Engineer"].ToString()+"接受了任务");
            htField.Add("ProcessTime", argsJsonData["CreateTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }
		#endregion

        #region //工程师处理任务工作流
        public EngineResponse ProcessedTask(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Receiver = argsJsonData["Receiver"].ToString();
            string Name = dbm.GetFieldValue("Sys_User", "Name", Receiver, "Id");//获取下一步接受者姓名，建单人姓名
            //修改工作刘表
            Hashtable htField = new Hashtable();
            string CaseId = argsJsonData["CaseId"].ToString();
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", Name);
            htField.Add("State", "Processed");
            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId",CaseId);
            int AccetptTask = dbm.Update("Cus_Workflow", htField, htCondition);

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", CaseId);
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            ////////添加工作流日志////////
            htCondition.Clear();
            htCondition.Add("Id",argsJsonData["Handler"].ToString());
            var HandlerName = dbm.GetDataTable("*", "Sys_User", htCondition, "Id");//处理人名字


            htField.Clear();
            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", Name);
            htField.Add("Action", WorkflowLogAction.Handled.ToString());
            htField.Add("Message", HandlerName.Rows[0]["Name"] + "处理了任务");
            htField.Add("ProcessTime", argsJsonData["FinishTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }

        /// <summary>
        /// 审核完成
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse CompleteTask(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Receiver = argsJsonData["Receiver"].ToString();
            //修改工作刘表
            Hashtable htField = new Hashtable();
            string CaseId = argsJsonData["CaseId"].ToString();
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", "无");
            htField.Add("State", "Complete");
            htField.Add("FinishTime",argsJsonData["FinishTime"].ToString());
            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId", CaseId);
            int AccetptTask = dbm.Update("Cus_Workflow", htField, htCondition);

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", CaseId);
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            ////////添加工作流日志////////

            htField.Clear();
            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", "无");

            htCondition.Clear();
            var HandlerName = dbm.GetFieldValue("Sys_User", "Name", argsJsonData["Handler"].ToString(), "Id");//处理人名字

            htField.Add("Action", WorkflowLogAction.Approve.ToString());
            htField.Add("Message", HandlerName + "审核了工单");
            htField.Add("ProcessTime", argsJsonData["FinishTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }
        #endregion


        /// <summary>
        /// 转办的方法
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse Zhuanban(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Receiver = argsJsonData["Receiver"].ToString();
            //修改工作刘表

            Hashtable htField = new Hashtable();
            string CaseId = argsJsonData["CaseId"].ToString();
            htField.Add("Receiver", Receiver);
            htField.Add("CreateTime", argsJsonData["CreateTime"].ToString());
            htField.Add("Engineer", argsJsonData["Receiver"].ToString());
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId", CaseId);
            int AccetptTask = dbm.Update("Cus_Workflow", htField, htCondition);

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", CaseId);
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            ////////添加工作流日志////////
            htCondition.Clear();


            htField.Clear();
            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", Receiver);

            var HandlerName = dbm.GetFieldValue("Sys_User", "Name", argsJsonData["Handler"].ToString(), "Id");//处理人名字

            htField.Add("Action", WorkflowLogAction.Transfer.ToString());
            htField.Add("Message", HandlerName + "把工单转给了" + Receiver + ",《转单原因》：" + argsJsonData["FinishTime"].ToString());
            htField.Add("ProcessTime", argsJsonData["CreateTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }


        /// <summary>
        /// 升级的方法
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse Shengji(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Receiver = argsJsonData["Receiver"].ToString();
            //修改工作刘表

            Hashtable htField = new Hashtable();
            string CaseId = argsJsonData["CaseId"].ToString();
            htField.Add("Receiver", Receiver);
            htField.Add("CreateTime", argsJsonData["CreateTime"].ToString());
            htField.Add("Engineer", argsJsonData["Receiver"].ToString());
            htField.Add("Handler", argsJsonData["Handler"].ToString());
            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId", CaseId);
            int AccetptTask = dbm.Update("Cus_Workflow", htField, htCondition);

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", CaseId);
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            ////////添加工作流日志////////
            htCondition.Clear();


            htField.Clear();
            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", Receiver);

            var HandlerName = dbm.GetFieldValue("Sys_User", "Name", argsJsonData["Handler"].ToString(), "Id");//处理人名字

            htField.Add("Action", WorkflowLogAction.LevelUp.ToString());
            htField.Add("Message", HandlerName + "把工单升级给了" + Receiver + ",《升级原因》：" + argsJsonData["FinishTime"].ToString());
            htField.Add("ProcessTime", argsJsonData["CreateTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse GoBack(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Receiver = argsJsonData["Receiver"].ToString();
            //修改工作刘表

            var ReceiverName = dbm.GetFieldValue("Sys_User", "Name", Receiver, "Id");//接收者人名字

            Hashtable htField = new Hashtable();
            string CaseId = argsJsonData["CaseId"].ToString();
            htField.Add("Receiver", ReceiverName);
            htField.Add("CreateTime", argsJsonData["CreateTime"].ToString());
            htField.Add("State", "GoBack");
            htField.Add("handler", argsJsonData["Handler"].ToString());

            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId", CaseId);

            int AccetptTask = dbm.Update("Cus_Workflow", htField, htCondition);

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", CaseId);
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            ////////添加工作流日志////////
            htCondition.Clear();


            htField.Clear();

            var HandlerName = dbm.GetFieldValue("Sys_User", "Name", argsJsonData["Handler"].ToString(), "Id");//处理人名字

            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", ReceiverName);
            htField.Add("Action", WorkflowLogAction.GoBack.ToString());
            htField.Add("Message", HandlerName + "把工单退回给了给了" + ReceiverName + ",客户反馈：" + argsJsonData["Engineer"].ToString() + argsJsonData["Creator"]);
            htField.Add("ProcessTime", argsJsonData["CreateTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);
        }
        
        /// <summary>
        /// 催办
        /// </summary>
        /// <param name="argsJsonData"></param>
        /// <returns></returns>
        public EngineResponse Cuiban(JsonData argsJsonData)
        {
            var dbm = DataBaseFactory.Instance.Create();
            var Handler = argsJsonData["Handler"].ToString();
            var CreateTime = argsJsonData["CreateTime"].ToString();

            Hashtable htCondition = new Hashtable();
            htCondition.Add("CaseId", argsJsonData["CaseId"].ToString());

            Hashtable htField = new Hashtable();
            htField.Add("Handler", Handler);
            htField.Add("CreateTime", CreateTime);

            int Cuiban = dbm.Update("Cus_Workflow", htField, htCondition);//他在这个环节报错

            ///////获取Cus_Workflow表中的Id
            htCondition.Clear();
            htCondition.Add("CaseId", argsJsonData["CaseId"].ToString());
            string Id = dbm.GetFieldValue("Cus_Workflow", "Id", htCondition, "");

            htField.Clear();
            htCondition.Clear();

            // 下面这三行，你干啥呢？转换，咋了，通过ID获取Name，你你要name干啥？下边有个message,我写说明
            var HandlerName = dbm.GetFieldValue("Sys_User", "Name", argsJsonData["Handler"].ToString(), "Id");//处理人名字
            var Receiver = dbm.GetFieldValue("Cus_Workflow", "Receiver", Id);//获取改工单的接收人（也就是谁正在处理）
            var State = dbm.GetFieldValue("Cus_Workflow", "State", Id);//获取改工单的状态

            htField.Add("WorkflowId", Id);
            htField.Add("handler", argsJsonData["Handler"].ToString());
            htField.Add("Receiver", Receiver);
            if (State == "Created") { htField.Add("Action", WorkflowLogAction.Created.ToString()); }
            if (State == "Processing") { htField.Add("Action", WorkflowLogAction.Handle.ToString()); }
            if (State == "GoBack") { htField.Add("Action", WorkflowLogAction.GoBack.ToString()); }
            
            htField.Add("Message", HandlerName + "在" + argsJsonData["CreateTime"] + ",催办了工单");
            htField.Add("ProcessTime", argsJsonData["CreateTime"].ToString());
            // 2.1 插入日志
            int logResult = dbm.Insert("Cus_WorkflowLog", htField);
            if (logResult < 1) return new EngineResponse(EngineResponse.Engine_Workflow_CreateWorkflowLogFail);

            return new EngineResponse(EngineResponse.Complete);


        }
    }
}
