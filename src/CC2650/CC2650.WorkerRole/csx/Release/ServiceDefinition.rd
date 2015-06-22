<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="CC2650.WorkerRole" generation="1" functional="0" release="0" Id="53e89560-517f-4304-8d45-e4e53c40e26b" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="CC2650.WorkerRoleGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="CC2650.XSocketsWorker:Endpoint1" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/LB:CC2650.XSocketsWorker:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="CC2650.XSocketsWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/MapCC2650.XSocketsWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="CC2650.XSocketsWorker:origin" defaultValue="">
          <maps>
            <mapMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/MapCC2650.XSocketsWorker:origin" />
          </maps>
        </aCS>
        <aCS name="CC2650.XSocketsWorker:uri" defaultValue="">
          <maps>
            <mapMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/MapCC2650.XSocketsWorker:uri" />
          </maps>
        </aCS>
        <aCS name="CC2650.XSocketsWorkerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/MapCC2650.XSocketsWorkerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:CC2650.XSocketsWorker:Endpoint1">
          <toPorts>
            <inPortMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorker/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapCC2650.XSocketsWorker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapCC2650.XSocketsWorker:origin" kind="Identity">
          <setting>
            <aCSMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorker/origin" />
          </setting>
        </map>
        <map name="MapCC2650.XSocketsWorker:uri" kind="Identity">
          <setting>
            <aCSMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorker/uri" />
          </setting>
        </map>
        <map name="MapCC2650.XSocketsWorkerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorkerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="CC2650.XSocketsWorker" generation="1" functional="0" release="0" software="C:\Users\Uffe\Documents\GitHub\TI-CC2650\src\CC2650\CC2650.WorkerRole\csx\Release\roles\CC2650.XSocketsWorker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="tcp" portRanges="8080" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="origin" defaultValue="" />
              <aCS name="uri" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;CC2650.XSocketsWorker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;CC2650.XSocketsWorker&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorkerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorkerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorkerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="CC2650.XSocketsWorkerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="CC2650.XSocketsWorkerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="CC2650.XSocketsWorkerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="d9143f05-12f9-43fc-a9ce-402e5aa13213" ref="Microsoft.RedDog.Contract\ServiceContract\CC2650.WorkerRoleContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="ec3a2167-b31e-41c3-8490-ba69181339b5" ref="Microsoft.RedDog.Contract\Interface\CC2650.XSocketsWorker:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/CC2650.WorkerRole/CC2650.WorkerRoleGroup/CC2650.XSocketsWorker:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>