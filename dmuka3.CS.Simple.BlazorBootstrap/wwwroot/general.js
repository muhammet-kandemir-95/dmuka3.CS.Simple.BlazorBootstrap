// #region Dmuka3
window.Dmuka3 = {
    'RunJS': function (js) {
        return new Function(js)();
    },
    'InvokeDotNet': function (assembly, method) {
        if (assembly === null || assembly === undefined) {
            assembly = 'dmuka3.CS.Simple.BlazorBootstrap';
        }

        if (method === null || method === undefined) {
            method = 'Dmuka3Table.JSEvent';
        }

        var args = [assembly, method];
        for (var i = 2; i < arguments.length; i++) {
            args.push(arguments[i]);
        }

        DotNet.invokeMethodAsync.apply(null, args).then(function () { });
    }
};
// #endregion


// #region Dmuka3Table
window.Dmuka3Table = {
    'Fill': function (tableId, rows) {
        var $table = $('#dmuka3-table-' + tableId);
        var uniqueId = $table.attr('data-unique-key');
        var $tableBody = $table.find('>tbody');
        $tableBody.html('');

        var $cloneRow = $('#dmuka3-table-clone-' + tableId);

        var $cloneRowBody = $cloneRow.find('> tr.data-body');
        if ($cloneRowBody.length === 0) {
            $cloneRowBody = $cloneRow.find('> tr:first-child');
        }

        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            (function (row) {
                var $row = $cloneRowBody.clone();
                var rowHtml = $row[0].outerHTML;

                for (var col in row) {
                    rowHtml = rowHtml.split('{{' + col + '}}').join(row[col]);
                }

                $row = $(rowHtml);

                $row.find('*[dom-events]').each(function (index, value) {
                    var $value = $(value);
                    var events = new Function('return ' + $value.attr('dom-events'))();
                    $value.removeAttr('dom-events');
                    for (var event in events) {
                        var eventName = events[event];

                        $value.on(event, function () {
                            if (typeof eventName === 'string') {
                                window.Dmuka3.InvokeDotNet(null, null, tableId, 'dmuka3-table-dom-events', eventName, row[uniqueId], JSON.stringify(row));
                            } else {
                                var args = [row[uniqueId], row, ...arguments];
                                eventName.apply(this, args);
                            }
                        });
                    }
                });

                $tableBody.append($row);
            })(row);
        }

        return true;
    },
    'Load': function (tableId) {
        var $table = $('#dmuka3-table-' + tableId);
        var $tableFoot = $table.find('>tfoot');

        var $cloneRow = $('#dmuka3-table-clone-' + tableId);

        var $cloneRowFoot = $cloneRow.find('> tr.data-foot');
        if ($cloneRowFoot.length === 0) {
            $cloneRowFoot = $cloneRow.find('> tr:nth-child(2)');
        }

        $tableFoot.append($cloneRowFoot);

        return true;
    }
};
// #endregion