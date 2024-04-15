namespace SnTsTypeGenerator.Services;

public class JsClassMapping
{
    public string JsClass { get; set; } = null!;

    public string PackageName { get; set; } = null!;

    public static IEnumerable<JsClassMapping> GetDefaultJsClassMappings()
    {
        yield return new JsClassMapping { JsClass = "GlideElement", PackageName = "com.glide.script.GlideElement" };
        yield return new JsClassMapping { JsClass = "GlideElementActionConditions", PackageName = "com.glide.script.glide_elements.GlideElementActionConditions" };
        yield return new JsClassMapping { JsClass = "GlideElementAudio", PackageName = "com.glide.db_audio.GlideElementAudio" };
        yield return new JsClassMapping { JsClass = "GlideElementBoolean", PackageName = "com.glide.script.glide_elements.GlideElementBoolean" };
        yield return new JsClassMapping { JsClass = "GlideElementBreakdownElement", PackageName = "com.snc.pa.dc.GlideElementBreakdownElement" };
        yield return new JsClassMapping { JsClass = "GlideElementGlideObject", PackageName = "com.glide.script.glide_elements.GlideElementGlideObject" };
        yield return new JsClassMapping { JsClass = "GlideElementCompressed", PackageName = "com.glide.script.glide_elements.GlideElementCompressed" };
        yield return new JsClassMapping { JsClass = "GlideElementConditions", PackageName = "com.glide.script.glide_elements.GlideElementConditions" };
        yield return new JsClassMapping { JsClass = "GlideElementCounter", PackageName = "com.glide.script.glide_elements.GlideElementCounter" };
        yield return new JsClassMapping { JsClass = "GlideElementCurrency", PackageName = "com.glide.currency.GlideElementCurrency" };
        yield return new JsClassMapping { JsClass = "GlideElementCurrency2", PackageName = "com.glide.currency2.GlideElementCurrency2" };
        yield return new JsClassMapping { JsClass = "GlideElementDataArray", PackageName = "com.snc.datastructure.GlideElementDataArray" };
        yield return new JsClassMapping { JsClass = "GlideElementDataObject", PackageName = "com.snc.datastructure.GlideElementDataObject" };
        yield return new JsClassMapping { JsClass = "GlideElementDataStructure", PackageName = "com.snc.datastructure.GlideElementDataStructure" };
        yield return new JsClassMapping { JsClass = "GlideElementNumeric", PackageName = "com.glide.script.glide_elements.GlideElementNumeric" };
        yield return new JsClassMapping { JsClass = "GlideElementDocumentation", PackageName = "com.glide.script.glide_elements.GlideElementDocumentation" };
        yield return new JsClassMapping { JsClass = "GlideElementDocumentId", PackageName = "com.glide.script.glide_elements.GlideElementDocumentId" };
        yield return new JsClassMapping { JsClass = "GlideElementDomainId", PackageName = "com.glide.script.glide_elements.GlideElementDomainId" };
        yield return new JsClassMapping { JsClass = "GlideElementFileAttachment", PackageName = "com.glide.script.glide_elements.GlideElementFileAttachment" };
        yield return new JsClassMapping { JsClass = "GlideElementGeoPoint", PackageName = "com.glide.script.glide_elements.GlideElementGeoPoint" };
        yield return new JsClassMapping { JsClass = "GlideElementGlideVar", PackageName = "com.glide.vars2.GlideElementGlideVar" };
        yield return new JsClassMapping { JsClass = "GlideElementGraphQLSchema", PackageName = "com.glide.graphql.element.GlideElementGraphQLSchema" };
        yield return new JsClassMapping { JsClass = "GlideElementIcon", PackageName = "com.glide.script.glide_elements.GlideElementIcon" };
        yield return new JsClassMapping { JsClass = "GlideElementInternalType", PackageName = "com.glide.script.glide_elements.GlideElementInternalType" };
        yield return new JsClassMapping { JsClass = "GlideElementIPAddress", PackageName = "com.glide.script.glide_elements.GlideElementIPAddress" };
        yield return new JsClassMapping { JsClass = "GlideElementNameValue", PackageName = "com.glide.glideobject.GlideElementNameValue" };
        yield return new JsClassMapping { JsClass = "GlideElementPassword", PackageName = "com.glide.script.glide_elements.GlideElementPassword" };
        yield return new JsClassMapping { JsClass = "GlideElementPassword2", PackageName = "com.glide.script.glide_elements.GlideElementPassword2" };
        yield return new JsClassMapping { JsClass = "GlideElementPhoneNumber", PackageName = "com.glide.PhoneNumber.GlideElementPhoneNumber" };
        yield return new JsClassMapping { JsClass = "GlideElementPrice", PackageName = "com.glide.currency.GlideElementPrice" };
        yield return new JsClassMapping { JsClass = "GlideElementReference", PackageName = "com.glide.script.glide_elements.GlideElementReference" };
        yield return new JsClassMapping { JsClass = "GlideElementRelatedTags", PackageName = "com.glide.labels.GlideElementRelatedTags" };
        yield return new JsClassMapping { JsClass = "GlideElementReplicationPayload", PackageName = "com.glide.script.glide_elements.GlideElementReplicationPayload" };
        yield return new JsClassMapping { JsClass = "GlideElementScript", PackageName = "com.glide.script.glide_elements.GlideElementScript" };
        yield return new JsClassMapping { JsClass = "GlideElementScript", PackageName = "com.glide.script.glide_elements.GlideElementScript" };
        yield return new JsClassMapping { JsClass = "GlideElementSimpleNameValue", PackageName = "com.glide.script.glide_elements.GlideElementSimpleNameValue" };
        yield return new JsClassMapping { JsClass = "GlideElementSnapshotTemplateValue", PackageName = "com.glide.script.glide_elements.GlideElementSnapshotTemplateValue" };
        yield return new JsClassMapping { JsClass = "GlideElementSourceId", PackageName = "com.glide.script.glide_elements.GlideElementSourceId" };
        yield return new JsClassMapping { JsClass = "GlideElementSourceName", PackageName = "com.glide.script.glide_elements.GlideElementSourceName" };
        yield return new JsClassMapping { JsClass = "GlideElementSourceTable", PackageName = "com.glide.script.glide_elements.GlideElementSourceTable" };
        yield return new JsClassMapping { JsClass = "GlideElementFullUTF8", PackageName = "com.glide.script.glide_elements.GlideElementFullUTF8" };
        yield return new JsClassMapping { JsClass = "GlideElementSysClassName", PackageName = "com.glide.script.glide_elements.GlideElementSysClassName" };
        yield return new JsClassMapping { JsClass = "GlideElementWorkflowConditions", PackageName = "com.glide.script.glide_elements.GlideElementWorkflowConditions" };
        yield return new JsClassMapping { JsClass = "GlideElementTranslatedField", PackageName = "com.glide.script.glide_elements.GlideElementTranslatedField" };
        yield return new JsClassMapping { JsClass = "GlideElementTranslatedHTML", PackageName = "com.glide.script.glide_elements.GlideElementTranslatedHTML" };
        yield return new JsClassMapping { JsClass = "GlideElementTranslatedText", PackageName = "com.glide.script.glide_elements.GlideElementTranslatedText" };
        yield return new JsClassMapping { JsClass = "GlideElementURL", PackageName = "com.glide.script.glide_elements.GlideElementURL" };
        yield return new JsClassMapping { JsClass = "GlideElementUserImage", PackageName = "com.glide.script.glide_elements.GlideElementUserImage" };
        yield return new JsClassMapping { JsClass = "GlideElementVariables", PackageName = "com.glide.vars2.GlideElementVariables" };
        yield return new JsClassMapping { JsClass = "GlideElementVariableConditions", PackageName = "com.glide.script.glide_elements.GlideElementVariableConditions" };
        yield return new JsClassMapping { JsClass = "GlideElementVariableTemplateValue", PackageName = "com.glide.script.glide_elements.GlideElementVariableTemplateValue" };
        yield return new JsClassMapping { JsClass = "GlideElementVideo", PackageName = "com.glide.script.glide_elements.GlideElementVideo" };
        yield return new JsClassMapping { JsClass = "GlideElementWikiText", PackageName = "com.glide.wiki.GlideElementWikiText" };
        yield return new JsClassMapping { JsClass = "GlideElementWorkflow", PackageName = "com.glide.stages.GlideElementWorkflow" };
        yield return new JsClassMapping { JsClass = "GlideElementWorkflowConditions", PackageName = "com.glide.script.glide_elements.GlideElementWorkflowConditions" };
        yield return new JsClassMapping { JsClass = "GlideElementScript", PackageName = "com.glide.script.glide_elements.GlideElementScript" };
    }
}
