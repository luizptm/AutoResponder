/***
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/

function TagsFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["CustomTagsFilterWidget"];
    };

    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };

    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadCustomers(); //load customer's list from the server
        this.registerEvents(); //handle events
    };

    this.renderWidget = function () {
        var html = '<p>Filtrar por Tags:</p>'
        html += '<select style="width:250px;" class="grid-filter-type form-control tagsList">'
        html += '<option value=""> -- selecione --</option>';
        html += '</select>';
        this.container.append(html);
    };

    /***
    * Method loads all customers from the server via Ajax:
    */
    this.loadCustomers = function () {
        var $this = this;
        /*
        $.post("/Users/GetTagsByJson/", function (data) {
            $this.fillTags(data.Items);
        });
        */
        $.ajax({
            url: "/Users/GetTagsByJson/",
            success: function (data, textStatus, jqXHR) {
                $this.fillTags(data.Items);
            }
        });
    };

    /***
    * Method fill tags select list by data
    */
    this.fillTags = function (items) {
        var list = this.container.find(".tagsList");
        for (var i = 0; i < items.length; i++) {
            list.append('<option ' + (items[i].Name == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i].Name + '">' + items[i].Name + '</option>');
        }
        list.append('</select>');
    };

    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        var list = this.container.find(".tagsList");
        //save current context:
        var $context = this;
        //register onclick event handler
        list.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 2 /* = Contains */ }];
            $context.cb(values);
        });
    };
}