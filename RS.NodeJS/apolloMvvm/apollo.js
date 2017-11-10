var apollo = {};
(function ($) {
    










})($);










apollo.MVVM = {
    ViewModel: Backbone.Epoxy.View,
    Model: Backbone.Epoxy.Model,
    Collection: Backbone.Collection,
    Service: Backbone.Service,
    Binding: Backbone.Epoxy.binding
};
apollo.MVVM.Model.prototype.echarts = function (attr, lab) {
    if (lab) {
        return [{ value: this.get(attr), name: this.get(lab) }]
    }
    return this.get(attr);
}


apollo.MVVM.Collection.prototype.echarts = function (attr, lab) {
    var tdata = this.pluck(attr);
    var tlab = this.pluck(lab);
    if (tlab.length > 0) {
        var arr = [];
        for (var i = 0; i < tdata.length; i++) {
            arr.push({
                value: tdata[i],
                name: tlab[i]
            })
        }
        return arr;
    }
    return [];
}