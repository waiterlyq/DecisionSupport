﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DSWeb.RWS {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RWS.IRService")]
    public interface IRService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRService/GenerDSTree", ReplyAction="http://tempuri.org/IRService/GenerDSTreeResponse")]
        bool GenerDSTree(string strModGUID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRService/GenerDSTree", ReplyAction="http://tempuri.org/IRService/GenerDSTreeResponse")]
        System.Threading.Tasks.Task<bool> GenerDSTreeAsync(string strModGUID);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRServiceChannel : DSWeb.RWS.IRService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RServiceClient : System.ServiceModel.ClientBase<DSWeb.RWS.IRService>, DSWeb.RWS.IRService {
        
        public RServiceClient() {
        }
        
        public RServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool GenerDSTree(string strModGUID) {
            return base.Channel.GenerDSTree(strModGUID);
        }
        
        public System.Threading.Tasks.Task<bool> GenerDSTreeAsync(string strModGUID) {
            return base.Channel.GenerDSTreeAsync(strModGUID);
        }
    }
}
