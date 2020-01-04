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
    /**
     * To fill table.
     * @param {any} tableId Dmuka3Table's unique id.
     * @param {any} rows New rows.
     * @returns {null} Result.
     */
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
                $row.removeAttr('data-body');
                var rowHtml = $row[0].outerHTML;

                for (var col in row) {
                    var $colDataAsText = $('<div></div>');
                    $colDataAsText.text(row[col]);

                    rowHtml = rowHtml.split('{{' + col + '}}').join($colDataAsText.html());
                }

                $row = $(rowHtml);

                $row.find('*[dom-events]').each(function (index, value) {
                    var $value = $(value);
                    var events = new Function('return ' + $value.attr('dom-events'))();
                    $value.removeAttr('dom-events');
                    for (var event in events) {
                        var eventValue = events[event];

                        if (event === 'load') {
                            if (typeof eventValue === 'string') {
                                window.Dmuka3.InvokeDotNet(null, null, tableId, 'dmuka3-table-dom-events', eventValue, row[uniqueId], JSON.stringify(row));
                            } else {
                                var args = [row[uniqueId], row];
                                eventValue.apply(this, args);
                            }
                        } else {
                            $value.on(event, function () {
                                if (typeof eventValue === 'string') {
                                    window.Dmuka3.InvokeDotNet(null, null, tableId, 'dmuka3-table-dom-events', eventValue, row[uniqueId], JSON.stringify(row));
                                } else {
                                    var args = [row[uniqueId], row, ...arguments];
                                    eventValue.apply(this, args);
                                }
                            });
                        }
                    }
                });

                $tableBody.append($row);
            })(row);
        }
    },
    /**
     * To do somethings which are neccessary while loading.
     * @param {any} tableId Dmuka3Table's unique id.
     * @returns {null} Result.
     */
    'Load': function (tableId) {
        var $table = $('#dmuka3-table-' + tableId);
        if ($table.data('__dmuka3table_isloaded') === true) {
            return true;
        }
        $table.data('__dmuka3table_isloaded', true);

        var $tableFoot = $table.find('>tfoot');

        var $cloneRow = $('#dmuka3-table-clone-' + tableId);

        var $cloneRowFoot = $cloneRow.find('> tr.data-foot');
        if ($cloneRowFoot.length === 0) {
            $cloneRowFoot = $cloneRow.find('> tr:nth-child(2)');
        }

        $tableFoot.append($cloneRowFoot);
    }
};
// #endregion

// #region Dmuka3Mask
window.Dmuka3Mask = {
    'Set': function (maskId, value) {
        var $mask = $('#dmuka3-mask-' + maskId);
        $mask.val(value);
    },
    'Validate': function (maskId) {
        var $mask = $('#dmuka3-mask-' + maskId);
        $mask.data('__dmuka3mask_inputEvent')();
    },
    'Load': function (maskId, pattern, requiredFilling, value) {
        var $mask = $('#dmuka3-mask-' + maskId);
        if ($mask.data('__dmuka3mask_isloaded') === true) {
            return;
        }
        $mask.data('__dmuka3mask_isloaded', true);

        $mask.val(value);

        var inputEvent = function (e) {
            var val = this.value;
            var newVal = '';
            var cursorIndex = this.selectionStart;
            var countMaskCharacter = 0;

            var maskIndex = 0;
            for (var i = 0; i < val.length; i++) {
                if (maskIndex >= pattern.length) {
                    break;
                }

                var c = val[i];
                var mask = pattern[maskIndex];
                while (maskIndex !== null) {
                    if (mask !== '?' && mask !== '9' && mask !== 'a' && mask !== 'A' && mask !== 'l' && mask !== 'L') {
                        newVal += mask;

                        if (c === mask) {
                            i++;

                            if (i >= val.length) {
                                break;
                            }

                            c = val[i];
                        } else {
                            if (i === cursorIndex - 1) {
                                countMaskCharacter++;
                            }
                        }

                        maskIndex++;

                        if (maskIndex >= pattern.length) {
                            break;
                        }
                    } else {
                        break;
                    }

                    mask = pattern[maskIndex];
                }

                if (i >= val.length) {
                    break;
                }

                if (maskIndex >= pattern.length) {
                    break;
                }

                mask = pattern[maskIndex];

                if (mask === '9') {
                    if (c >= '0' && c <= '9') {
                        newVal += c;
                        maskIndex++;
                    }
                } else if (mask === 'a' || mask === 'A') {
                    if (c.toUpperCase() !== c || c.toLowerCase() !== c) {
                        newVal += c;
                        maskIndex++;
                    }
                } else if (mask === '?') {
                    newVal += c;
                    maskIndex++;
                } else if (mask === 'l') {
                    if (c.toUpperCase() !== c && c.toLowerCase() === c) {
                        newVal += c;
                        maskIndex++;
                    }
                } else if (mask === 'L') {
                    if (c.toLowerCase() !== c && c.toUpperCase() === c) {
                        newVal += c;
                        maskIndex++;
                    }
                }
            }

            if (val.length === cursorIndex) {
                cursorIndex += 1;
            }
            cursorIndex += countMaskCharacter;

            this.value = newVal;
            if (this.value.length > cursorIndex) {
                this.selectionStart = cursorIndex;
                this.selectionEnd = cursorIndex;
            }
        };
        $mask.data('__dmuka3mask_inputEvent', function () {
            inputEvent.call($mask[0], { preventDefault: function () { } });
        });
        $mask.data('__dmuka3mask_inputEvent')();

        $mask.on('input', inputEvent);

        if (requiredFilling === true) {
            if ($mask.val().length !== pattern.length) {
                $mask.data('__dmuka3mask_previousValue', this.value);
                $mask.val('');
            }

            $mask.on('focusout', function (e) {
                if (this.value.length !== pattern.length) {
                    $mask.data('__dmuka3mask_previousValue', this.value);
                    this.value = '';
                    $mask.trigger('change');
                }
            }).on('focus', function (e) {
                if (this.value.length === 0) {
                    this.value = $mask.data('__dmuka3mask_previousValue');
                    this.selectionStart = this.value.length;
                    this.selectionEnd = this.value.length;
                }
            });
        }
    }
};
// #endregion