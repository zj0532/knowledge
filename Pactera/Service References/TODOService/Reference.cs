﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.0
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pactera.TODOService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://10.20.0.26:8080/services/TODOService", ConfigurationName="TODOService.TODOService")]
    public interface TODOService {
        
        // CODEGEN: 消息 mainRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.mainResponse main(Pactera.TODOService.mainRequest request);
        
        // CODEGEN: 消息 AddTODOListRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.AddTODOListResponse AddTODOList(Pactera.TODOService.AddTODOListRequest request);
        
        // CODEGEN: 消息 AddToDoRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.AddToDoResponse AddToDo(Pactera.TODOService.AddToDoRequest request);
        
        // CODEGEN: 消息 UpdateTODOStatusRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.UpdateTODOStatusResponse UpdateTODOStatus(Pactera.TODOService.UpdateTODOStatusRequest request);
        
        // CODEGEN: 消息 UpdateTODOStatusAllRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.UpdateTODOStatusAllResponse UpdateTODOStatusAll(Pactera.TODOService.UpdateTODOStatusAllRequest request);
        
        // CODEGEN: 消息 DeleteTODOByPersonRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.DeleteTODOByPersonResponse DeleteTODOByPerson(Pactera.TODOService.DeleteTODOByPersonRequest request);
        
        // CODEGEN: 消息 DeleteTODOByReqidRequest 的包装命名空间(http://WS.cnqc.software.com)以后生成的消息协定与默认值(http://10.20.0.26:8080/services/TODOService)不匹配
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        Pactera.TODOService.DeleteTODOByReqidResponse DeleteTODOByReqid(Pactera.TODOService.DeleteTODOByReqidRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="main", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class mainRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string[] args;
        
        public mainRequest() {
        }
        
        public mainRequest(string[] args) {
            this.args = args;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="mainResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class mainResponse {
        
        public mainResponse() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AddTODOList", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class AddTODOListRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string title;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public string url;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=2)]
        public string softName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=3)]
        public string corpId;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=4)]
        public string corpName;
        
        public AddTODOListRequest() {
        }
        
        public AddTODOListRequest(string title, string url, string softName, string corpId, string corpName) {
            this.title = title;
            this.url = url;
            this.softName = softName;
            this.corpId = corpId;
            this.corpName = corpName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AddTODOListResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class AddTODOListResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string AddTODOListReturn;
        
        public AddTODOListResponse() {
        }
        
        public AddTODOListResponse(string AddTODOListReturn) {
            this.AddTODOListReturn = AddTODOListReturn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AddToDo", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class AddToDoRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string jsonString;
        
        public AddToDoRequest() {
        }
        
        public AddToDoRequest(string jsonString) {
            this.jsonString = jsonString;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AddToDoResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class AddToDoResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string AddToDoReturn;
        
        public AddToDoResponse() {
        }
        
        public AddToDoResponse(string AddToDoReturn) {
            this.AddToDoReturn = AddToDoReturn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateTODOStatus", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class UpdateTODOStatusRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string jsonString;
        
        public UpdateTODOStatusRequest() {
        }
        
        public UpdateTODOStatusRequest(string jsonString) {
            this.jsonString = jsonString;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateTODOStatusResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class UpdateTODOStatusResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string UpdateTODOStatusReturn;
        
        public UpdateTODOStatusResponse() {
        }
        
        public UpdateTODOStatusResponse(string UpdateTODOStatusReturn) {
            this.UpdateTODOStatusReturn = UpdateTODOStatusReturn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateTODOStatusAll", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class UpdateTODOStatusAllRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string jsonString;
        
        public UpdateTODOStatusAllRequest() {
        }
        
        public UpdateTODOStatusAllRequest(string jsonString) {
            this.jsonString = jsonString;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateTODOStatusAllResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class UpdateTODOStatusAllResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string UpdateTODOStatusAllReturn;
        
        public UpdateTODOStatusAllResponse() {
        }
        
        public UpdateTODOStatusAllResponse(string UpdateTODOStatusAllReturn) {
            this.UpdateTODOStatusAllReturn = UpdateTODOStatusAllReturn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DeleteTODOByPerson", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class DeleteTODOByPersonRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string jsonString;
        
        public DeleteTODOByPersonRequest() {
        }
        
        public DeleteTODOByPersonRequest(string jsonString) {
            this.jsonString = jsonString;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DeleteTODOByPersonResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class DeleteTODOByPersonResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string DeleteTODOByPersonReturn;
        
        public DeleteTODOByPersonResponse() {
        }
        
        public DeleteTODOByPersonResponse(string DeleteTODOByPersonReturn) {
            this.DeleteTODOByPersonReturn = DeleteTODOByPersonReturn;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DeleteTODOByReqid", WrapperNamespace="http://WS.cnqc.software.com", IsWrapped=true)]
    public partial class DeleteTODOByReqidRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string jsonString;
        
        public DeleteTODOByReqidRequest() {
        }
        
        public DeleteTODOByReqidRequest(string jsonString) {
            this.jsonString = jsonString;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DeleteTODOByReqidResponse", WrapperNamespace="http://10.20.0.26:8080/services/TODOService", IsWrapped=true)]
    public partial class DeleteTODOByReqidResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string DeleteTODOByReqidReturn;
        
        public DeleteTODOByReqidResponse() {
        }
        
        public DeleteTODOByReqidResponse(string DeleteTODOByReqidReturn) {
            this.DeleteTODOByReqidReturn = DeleteTODOByReqidReturn;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TODOServiceChannel : Pactera.TODOService.TODOService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TODOServiceClient : System.ServiceModel.ClientBase<Pactera.TODOService.TODOService>, Pactera.TODOService.TODOService {
        
        public TODOServiceClient() {
        }
        
        public TODOServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TODOServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TODOServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TODOServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.mainResponse Pactera.TODOService.TODOService.main(Pactera.TODOService.mainRequest request) {
            return base.Channel.main(request);
        }
        
        public void main(string[] args) {
            Pactera.TODOService.mainRequest inValue = new Pactera.TODOService.mainRequest();
            inValue.args = args;
            Pactera.TODOService.mainResponse retVal = ((Pactera.TODOService.TODOService)(this)).main(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.AddTODOListResponse Pactera.TODOService.TODOService.AddTODOList(Pactera.TODOService.AddTODOListRequest request) {
            return base.Channel.AddTODOList(request);
        }
        
        public string AddTODOList(string title, string url, string softName, string corpId, string corpName) {
            Pactera.TODOService.AddTODOListRequest inValue = new Pactera.TODOService.AddTODOListRequest();
            inValue.title = title;
            inValue.url = url;
            inValue.softName = softName;
            inValue.corpId = corpId;
            inValue.corpName = corpName;
            Pactera.TODOService.AddTODOListResponse retVal = ((Pactera.TODOService.TODOService)(this)).AddTODOList(inValue);
            return retVal.AddTODOListReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.AddToDoResponse Pactera.TODOService.TODOService.AddToDo(Pactera.TODOService.AddToDoRequest request) {
            return base.Channel.AddToDo(request);
        }
        
        public string AddToDo(string jsonString) {
            Pactera.TODOService.AddToDoRequest inValue = new Pactera.TODOService.AddToDoRequest();
            inValue.jsonString = jsonString;
            Pactera.TODOService.AddToDoResponse retVal = ((Pactera.TODOService.TODOService)(this)).AddToDo(inValue);
            return retVal.AddToDoReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.UpdateTODOStatusResponse Pactera.TODOService.TODOService.UpdateTODOStatus(Pactera.TODOService.UpdateTODOStatusRequest request) {
            return base.Channel.UpdateTODOStatus(request);
        }
        
        public string UpdateTODOStatus(string jsonString) {
            Pactera.TODOService.UpdateTODOStatusRequest inValue = new Pactera.TODOService.UpdateTODOStatusRequest();
            inValue.jsonString = jsonString;
            Pactera.TODOService.UpdateTODOStatusResponse retVal = ((Pactera.TODOService.TODOService)(this)).UpdateTODOStatus(inValue);
            return retVal.UpdateTODOStatusReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.UpdateTODOStatusAllResponse Pactera.TODOService.TODOService.UpdateTODOStatusAll(Pactera.TODOService.UpdateTODOStatusAllRequest request) {
            return base.Channel.UpdateTODOStatusAll(request);
        }
        
        public string UpdateTODOStatusAll(string jsonString) {
            Pactera.TODOService.UpdateTODOStatusAllRequest inValue = new Pactera.TODOService.UpdateTODOStatusAllRequest();
            inValue.jsonString = jsonString;
            Pactera.TODOService.UpdateTODOStatusAllResponse retVal = ((Pactera.TODOService.TODOService)(this)).UpdateTODOStatusAll(inValue);
            return retVal.UpdateTODOStatusAllReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.DeleteTODOByPersonResponse Pactera.TODOService.TODOService.DeleteTODOByPerson(Pactera.TODOService.DeleteTODOByPersonRequest request) {
            return base.Channel.DeleteTODOByPerson(request);
        }
        
        public string DeleteTODOByPerson(string jsonString) {
            Pactera.TODOService.DeleteTODOByPersonRequest inValue = new Pactera.TODOService.DeleteTODOByPersonRequest();
            inValue.jsonString = jsonString;
            Pactera.TODOService.DeleteTODOByPersonResponse retVal = ((Pactera.TODOService.TODOService)(this)).DeleteTODOByPerson(inValue);
            return retVal.DeleteTODOByPersonReturn;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Pactera.TODOService.DeleteTODOByReqidResponse Pactera.TODOService.TODOService.DeleteTODOByReqid(Pactera.TODOService.DeleteTODOByReqidRequest request) {
            return base.Channel.DeleteTODOByReqid(request);
        }
        
        public string DeleteTODOByReqid(string jsonString) {
            Pactera.TODOService.DeleteTODOByReqidRequest inValue = new Pactera.TODOService.DeleteTODOByReqidRequest();
            inValue.jsonString = jsonString;
            Pactera.TODOService.DeleteTODOByReqidResponse retVal = ((Pactera.TODOService.TODOService)(this)).DeleteTODOByReqid(inValue);
            return retVal.DeleteTODOByReqidReturn;
        }
    }
}
