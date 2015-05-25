//-----------------------------------------------------------------------------------------------------

// instanciate Doc object and initalize variable 
// depending on type (IH1600.enum.contentType)
function docFactory(oListItem, type) {
    var doc = new Doc();
    if (oListItem != null && type == IH1600.enum.contentType.DEP) {        
        doc.codeIh1600 = oListItem.get_item(IH1600.field.info.Codification.internalName);
        doc.id = oListItem.get_id();
        doc.title = oListItem.get_item(IH1600.field.info.Title.internalName);
        doc.status = oListItem.get_item(IH1600.field.info.StatusCurrent.internalName);
        doc.site = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Site.internalName));
        doc.projet = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Projet.internalName));
        doc.type = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Typologie.internalName));
        doc.version = oListItem.get_item(IH1600.field.info.Indice.internalName);
        doc.revision = oListItem.get_item(IH1600.field.info.EDFRevision.internalName);
        doc.verificationStatus = oListItem.get_item(IH1600.field.info.Verification.internalName);
        doc.item;
        doc.type;
        doc.filename;
        doc.fileurl;
        doc.getFileInfo();
    }
    else if (oListItem != null && type == IH1600.enum.contentType.REF) {
        doc.codeIh1600 = oListItem.get_item(IH1600.field.info.Codification.internalName);
        doc.id = oListItem.get_id();
        doc.title = oListItem.get_item(IH1600.field.info.Title.internalName);
        doc.status = IH1600.enum.status.BPE;
        doc.site = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Site.internalName));
        doc.projet = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Projet.internalName));
        doc.type = new Term(RNVO.common.ensureValue(oListItem, IH1600.field.info.Typologie.internalName));
        doc.version = oListItem.get_item(IH1600.field.info.Indice.internalName);
        doc.revision = "";
        doc.verificationStatus = "";
        doc.item;
        doc.type;
        doc.filename;
        doc.fileurl;
        doc.getFileInfo();
    }
    
    return doc;
}

// constructor Doc Object
function Doc() {
    /// <param name='oListItem' type='SP.ListItem' />
    this.codeIh1600;
    this.id;
    this.title;
    this.status;
    this.site;
    this.projet;
    this.type;
    this.version;
    this.revision;
    this.verificationStatus;
    this.item;
    this.type;
    this.filename;
    this.fileurl;
    this.obs = new ObsCollection();
    this.loaded = false;
}

Doc.prototype.isNewFile = function () {
    return (this.version == undefined && this.revision == undefined);
}

Doc.prototype.getExpectedStatus = function () {
    if (this.status == IH1600.enum.status.BPE) return IH1600.enum.status.PREL;
    if (this.verificationStatus == IH1600.enum.validation.VSO || this.verificationStatus == IH1600.enum.validation.VSOSV) return IH1600.enum.status.BPE;
    else return IH1600.enum.status.PREL;
}

Doc.prototype.getNextVersion = function () {
    if (this.version == "" || this.version == null) return "A";
    else if (this.status == IH1600.enum.status.BPE) return String.fromCharCode(this.version.charCodeAt(0) + 1);
    else return this.version;
}

Doc.prototype.getNextRevision = function (forcePREL) {

    if (this.revision == "" || this.revision == null || this.status == IH1600.enum.status.BPE) {
        // it's a new version, either first uplaod ever or new version after BPE, so starting from 0
        return "0";
    }

    var force = (forcePREL != undefined || forcePREL == true);

    if (!force && (this.verificationStatus == IH1600.enum.validation.VSO || this.verificationStatus == IH1600.enum.validation.VSOSV)) {
        // it's a BPE Version, so only letter. Revision is blank
        return "";
    }
    else {
        // it's a regular new PREL version, just increment current revision
        var r = parseInt(this.revision);
        r++;
        return r;
    }
}

Doc.prototype.getFileInfo = function () {
    /// <summary>Get info about the actual file</summary>   
    var _self = this;
    var spItem = IH1600.core.library.getItemById(this.id)
    var spFile = spItem.get_file();
    IH1600.core.context.load(spItem);
    IH1600.core.context.load(spFile);
    IH1600.core.context.executeQueryAsync(
        function () {
            _self.filename = spFile.get_name();
            _self.fileurl = spFile.get_serverRelativeUrl();
        },
        RNVO.common.onQueryFailed
    );
};

Doc.prototype.getObs = function () {

    var _self = this;
    var camlQuery = new SP.CamlQuery();
    // this suppose that the doc item have the same title value as the name field value !!
    var xml = '<View><Query>' +
                        '<Where>' +
                            '<And>' +
                                '<And>' +
                                    '<Eq>' +
                                        '<FieldRef Name="EDFRevision" />' +
                                        '<Value Type="Text">' + this.revision + '</Value>' +
                                    '</Eq>' +
                                    '<Eq>' +
                                        '<FieldRef Name="EDFVersion" />' +
                                        '<Value Type="Text">' + this.version + '</Value>' +
                                    '</Eq>' +
                                '</And>' +
                                '<Eq>' +
                                    '<FieldRef Name=\'document\' />' +
                                    '<Value Type=\'Lookup\'>' + this.title + '</Value>' +
                                 '</Eq>' +
                             '</And>' +
                        '</Where>' +
                        '<OrderBy><FieldRef Name=\'Created\' Ascending=\'FALSE\' /></OrderBy>' +
                    '</Query>' +
                '<RowLimit>1</RowLimit>' +
              '</View>';
    camlQuery.set_viewXml(xml);
    var listItemColl = IH1600.core.obsList.getItems(camlQuery);
    IH1600.core.context.load(listItemColl);
    IH1600.core.context.executeQueryAsync(function (sender, args) {
        _self.loaded = true;
        var listEnum = listItemColl.getEnumerator();
        while (listEnum.moveNext()) {
            var oListItem = listEnum.get_current();
            _self.obs.add(new Obs(oListItem));
        }
    },
        RNVO.common.onQueryFailed
    );
}

//-----------------------------------------------------------------------------------------------------

function DocCollection() {
    this.docs = [];
}

DocCollection.prototype.add = function (doc) {
    if (doc != null && doc != undefined) this.docs.push(doc);
}

DocCollection.prototype.getByCodeIh = function (id) {
    /// <summary>tells whether a filename is in the collection</summary>
    /// <param name="filename" type="string">name of the file to search for</param>
    /// <return type="bool">if the file is found</return>
    var result = null;
    for (var i = 0; i < this.docs.length; i++) {
        if (this.docs[i].id == id) {
            result = this.docs[i];
            break;
        }
    }
    return result;
}

DocCollection.prototype.hasCodification = function (code) {
    return Object.keys(this.getCodeIh()).indexOf(code) > -1
}

DocCollection.prototype.getCodeIh = function () {
    var result = [];
    for (var i = 0; i < this.docs.length; i++) {
        var current = this.docs[i].codeIh1600;
        if (current != null) result.push(current);
    }
    return result;
}

DocCollection.prototype.getByFileName = function (filename) {
    /// <summary>tells whether a filename is in the collection</summary>
    /// <param name="filename" type="string">name of the file to search for</param>
    /// <return type="bool">if the file is found</return>
    var result = null;
    for (var i = 0; i < this.docs.length; i++) {
        if (this.docs[i].filename == filename) {
            result = this.docs[i];
            break;
        }
    }
    return result;
}

//-----------------------------------------------------------------------------------------------------

function FieldInfo(fname, internalName, taxon, selector) {
    this.controlId = null;
    this.fieldName = fname;
    this.internalName = internalName;
    this.isTaxon = taxon;
    this.hasValue = false;
    this.selector = selector;
}

//-----------------------------------------------------------------------------------------------------
function Obs(oListItem) {
    this.id = oListItem.get_id();
    this.title = oListItem.get_item("Title");
    this.obs = oListItem.get_item("observation");
    //this.validation = oListItem.get_item("validation");
}
//-----------------------------------------------------------------------------------------------------
function ObsCollection() {
    this.obsColl = [];
}

ObsCollection.prototype.add = function (o) {
    this.obsColl.push(o);
}

//-----------------------------------------------------------------------------------------------------
function Term(parentColl) {
    this.parentCollection = parentColl;
}

Term.prototype.setValue = function (obj, parent_term) {
    if (obj != null && obj.constructor.__typeName == "SP.Taxonomy.TaxonomyFieldValue") {
        this.termGuid = obj.get_termGuid();
        this.label = obj.get_label();
        this.typeId = obj.get_typeId();
        this.wssId = obj.get_wssId();
        this.childs = new TermCollection();
        this.spObject = obj;
    }
    else if (obj != null && obj.constructor.__typeName == "SP.Taxonomy.Term") {
        this.termGuid = obj.get_id();
        this.label = obj.get_name();
        this.spObject = obj;
        //this.typeId = obj.get_typeId();
        //this.wssId = obj.get_wssId();
    }

    this.parentTerm = parent_term;
    this.jObject = null;
}

Term.prototype.add = function (term) {
    this.childs.add(term);
}

Term.prototype.hasChild = function () {
    return this.childs.count() > 0;
}

Term.prototype.toString = function () {
    return this.label;
};


SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('rnvo.ih1600.model');

//# sourceURL=rnvo.ih1600.model.js

