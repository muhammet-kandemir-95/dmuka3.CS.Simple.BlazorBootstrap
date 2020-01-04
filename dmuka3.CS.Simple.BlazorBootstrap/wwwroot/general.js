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
     */
    'Load': function (tableId) {
        var $table = $('#dmuka3-table-' + tableId);
        if ($table.data('__dmuka3table_isloaded') === true) {
            return;
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
    /**
     * To set input's value.
     * @param {any} maskId Dmuka3Mask's unique id.
     * @param {any} value New value.
     * @param {any} previousValue Previous value before formatting.
     */
    'Set': function (maskId, value, previousValue) {
        var $mask = $('#dmuka3-mask-' + maskId);
        $mask.val(value);
        $mask.data('__dmuka3mask_previousValue', previousValue);
        $mask.data('__dmuka3mask_inputEvent')();
    },
    /**
     * To do somethings which are neccessary while loading.
     * @param {any} maskId Dmuka3Mask's unique id.
     * @param {any} pattern Mask's pattern.
     * @param {any} requiredFilling Mask's required filling.
     * @param {any} value Default value.
     * @param {any} previousValue Previous value before formatting.
     */
    'Load': function (maskId, pattern, requiredFilling, value, previousValue) {
        var $mask = $('#dmuka3-mask-' + maskId);
        if ($mask.data('__dmuka3mask_isloaded') === true) {
            return;
        }
        $mask.data('__dmuka3mask_isloaded', true);

        $mask.data('__dmuka3mask_previousValue', previousValue);
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
            $mask.on('focusout', function (e) {
                if (this.value.length !== pattern.length) {
                    this.value = '';
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

// #region Dmuka3Number
window.Dmuka3Number = {
    /**
     * To set input's value.
     * @param {any} numberId Dmuka3Number's unique id.
     * @param {any} value New value.
     */
    'Set': function (numberId, value) {
        var $number = $('#dmuka3-number-' + numberId);
        $number.val(value);
        $number.data('__dmuka3number_inputEvent')();
    },
    /**
     * To do somethings which are neccessary while loading.
     * @param {any} numberId Dmuka3Number's unique id.
     * @param {any} format Will value be formatted?
     * @param {any} formatCharacters Format characters.
     * @param {any} decimalPlaces Decimal places.
     * @param {any} value Default value.
     */
    'Load': function (numberId, format, formatCharacters, decimalPlaces, value) {
        var $number = $('#dmuka3-number-' + numberId);
        if ($number.data('__dmuka3number_isloaded') === true) {
            return;
        }
        $number.data('__dmuka3number_isloaded', true);

        $number.val(value);

        var inputEvent = function (e) {
            var val = this.value;
            var newVal = '';
            var cursorIndex = this.selectionStart;

            var dotExist = false;
            var decimalPlacesCounter = 0;
            var pc = null;
            for (var i = 0; i < val.length; i++) {
                var c = val[i];

                if (c >= '0' && c <= '9') {
                    if (decimalPlacesCounter < decimalPlaces || decimalPlaces <= 0) {
                        newVal += c;

                        if (dotExist === true) {
                            decimalPlacesCounter++;
                        }
                    }
                } else if (format !== false) {
                    if (c === formatCharacters[0]) {
                        if (newVal.length !== 0 && c !== pc && dotExist === false) {
                            newVal += c;
                        } else if (i <= cursorIndex) {
                            cursorIndex = Math.max(0, cursorIndex - 1);
                        }
                    } else if (c === formatCharacters[1]) {
                        if (dotExist === false && pc !== formatCharacters[0] && decimalPlaces > 0) {
                            newVal += c;
                        } else if (i <= cursorIndex) {
                            cursorIndex = Math.max(0, cursorIndex - 1);
                        }
                        dotExist = true;
                    }
                } else {
                    if (c === '.') {
                        if (dotExist === false) {
                            newVal += c;
                        }
                        dotExist = true;
                    }
                }

                if (newVal.length > 0) {
                    pc = newVal[newVal.length - 1];
                }
            }

            if (format !== false) {
                var iop = newVal.indexOf(formatCharacters[1]);
                var dotCounter = 4;
                val = newVal;
                if (iop >= 0) {
                    newVal = newVal.substr(iop);
                } else {
                    newVal = '';
                    iop = val.length;
                }

                for (var j = iop - 1; j >= 0; j--) {
                    var d = val[j];

                    if (dotCounter > 4) {
                        dotCounter = 4;
                    }
                    dotCounter--;
                    if (dotCounter < 0) {
                        dotCounter = 4;
                    }

                    if (d === formatCharacters[0]) {
                        if (dotCounter === 0) {
                            newVal = d + newVal;
                            dotCounter = 4;
                        } else if (j <= cursorIndex) {
                            dotCounter++;
                            cursorIndex = Math.max(0, cursorIndex - 1);;
                        }
                    } else {
                        if (dotCounter === 0) {
                            newVal = formatCharacters[0] + newVal;
                            if (j <= cursorIndex) {
                                cursorIndex++;
                            }
                            dotCounter = 3;
                        }
                        newVal = d + newVal;
                    }
                }
            }

            if (val.length === cursorIndex) {
                cursorIndex += 1;
            }

            if (this.value.length + 1 === newVal.length && newVal.length > 0 && (newVal[cursorIndex - 1] === formatCharacters[0] || newVal[cursorIndex - 1] === formatCharacters[1])) {
                cursorIndex = Math.max(0, cursorIndex - 1);
            }

            this.value = newVal;
            if (this.value.length > cursorIndex) {
                this.selectionStart = cursorIndex;
                this.selectionEnd = cursorIndex;
            }
        };
        $number.data('__dmuka3number_inputEvent', function () {
            inputEvent.call($number[0], { preventDefault: function () { } });
        });
        $number.data('__dmuka3number_inputEvent')();

        $number.on('input', inputEvent);
    }
};
// #endregion