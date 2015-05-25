

SPDO.taxonomy.getTermSet = function (setName) {
    var setCollection = SPDO.Taxonomy.Groups[0];
    for (var i = 0; i < setCollection.length; i++) {    
        if (setCollection[i].Name == setName) return setCollection[i];
    }    
};

SPDO.taxonomy.getTerm = function (setName, termName) {
    var set = SPDO.taxonomy.getTermSet(setName);
    
}
