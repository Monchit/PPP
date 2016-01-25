$(function(){
    denySelectionOnDoubleClick = function ($el) {
        // see http://stackoverflow.com/questions/2132172/disable-text-highlighting-on-double-click-in-jquery/2132230#2132230
        if ($.browser.mozilla) {//Firefox
            $el.css('MozUserSelect', 'none');
        } else if ($.browser.msie) {//IE
            $el.bind('selectstart', function () {
                return false;
            });
        } else {//Opera, etc.
            $el.mousedown(function () {
                return false;
            });
        }
    };
    inColumnHeader = function (text, columnHeaders) {
        var i = 0, length = columnHeaders.length;
        for (; i < length; i++) {
            if (columnHeaders[i].startColumnName === text) {
                return i;
            }
        }
        
        return -1;
    };
    insertColumnGroupHeaders = function ($mygrid, useColSpanStyle, columnGroupHeaders) {
        var ts = $mygrid[0], p = ts.p, i, cmi, skip = 0, $tr, $colHeader, th, $th, thStyle,
            iCol,
            cghi,
            startColumnName,
            numberOfColumns,
            titleText,
            cVisibleColumns,
            colModel = p.colModel,
            cml = colModel.length,
            ths = ts.grid.headers,
            $gview = $mygrid.closest("div.ui-jqgrid-view"),
            $gbox = $gview.parent(),
            $bdiv = $gview.children("div.ui-jqgrid-bdiv"),
            $hdiv = $gview.children("div.ui-jqgrid-hdiv"),
            $htable = $hdiv.children("div.ui-jqgrid-hbox").children("table.ui-jqgrid-htable"),
            $trLabels = $htable.children("thead").children("tr.ui-jqgrid-labels:last"),
            $thead = $htable.children("thead"),
            $theadInTable,
            originalResizeStop,
            $firstHeaderRow = $('<tr>', { role: "row", "aria-hidden": "true" }).addClass("jqg-first-row-header").css("height", "auto"),
            $firstRow;

        $mygrid.prepend($thead);
        $tr = $('<tr>', { role: "rowheader" }).addClass("ui-jqgrid-labels");
        for (i = 0; i < cml; i++) {
            th = ths[i].el;
            $th = $(th);
            denySelectionOnDoubleClick($th); // needed for Firefox to prevent selection on doubleclick
            cmi = colModel[i];

            // build the next cell for the first header row
            thStyle = { height: '0px', width: ths[i].width + 'px', display: (cmi.hidden ? 'none' : '') };
            $("<th>", { role: 'gridcell' }).css(thStyle).appendTo($firstHeaderRow);

            th.style.width = ""; // remove unneeded style
            iCol = inColumnHeader(cmi.name, columnGroupHeaders);
            if (iCol >= 0) {
                cghi = columnGroupHeaders[iCol];
                startColumnName = cghi.startColumnName;
                numberOfColumns = cghi.numberOfColumns;
                titleText = cghi.titleText;

                // caclulate the number of visible columns from the next numberOfColumns columns
                for (cVisibleColumns = 0, iCol = 0; iCol < numberOfColumns && (i + iCol < cml) ; iCol++) {
                    if (!colModel[i + iCol].hidden) {
                        cVisibleColumns++;
                    }
                }

                // The next numberOfColumns headers will be moved in the next row
                // in the current row will be placed the new column header with the titleText.
                // The text will be over the cVisibleColumns columns
                $colHeader = $('<th>', { colspan: String(cVisibleColumns), role: "columnheader" })
                    .addClass("ui-state-default ui-th-column-header ui-th-ltr")
                    .html(titleText);
                if (p.headertitles) {
                    $colHeader.attr("title", $colHeader.text());
                }
                denySelectionOnDoubleClick($colHeader);

                $th.before($colHeader); // insert new column header before the current
                $tr.append(th);         // move the current header in the next row

                // set the coumter of headers which will be moved in the next row
                skip = numberOfColumns - 1;
            } else {
                if (skip === 0) {
                    if (useColSpanStyle) {
                        // expand the header height to two rows
                        $th.attr("rowspan", "2");
                    } else {
                        $('<th>', { role: "columnheader" })
                            .addClass("ui-state-default ui-th-column-header")
                            .css("display", cmi.hidden ? 'none' : '')
                            .insertBefore($th);
                        $tr.append(th);
                    }
                } else {
                    // move the header to the next row
                    $th.css({ "padding-top": "2px", height: "19px" });
                    $tr.append(th);
                    skip--;
                }
            }
        }
        $theadInTable = $mygrid.children("thead");
        $theadInTable.prepend($firstHeaderRow);
        $tr.insertAfter($trLabels);
        $htable.append($theadInTable);

        if (useColSpanStyle) {
            // Increase the height of resizing span of visible headers
            $htable.find("span.ui-jqgrid-resize").each(function () {
                var $parent = $(this).parent();
                if ($parent.is(":visible")) {
                    this.style.cssText = 'height: ' + $parent.height() + 'px !important; cursor: col-resize;';
                }
            });

            // Set position of the sortable div (the main lable)
            // with the column header text to the middle of the cell.
            // One should not do this for hidden headers.
            $htable.find("div.ui-jqgrid-sortable").each(function () {
                var $ts = $(this), $parent = $ts.parent();
                if ($parent.is(":visible")) {
                    $ts.css('top', ($parent.height() - $ts.outerHeight()) / 2 + 'px');
                }
            });
        }

        // Preserve original resizeStop event if any defined
        if ($.isFunction(p.resizeStop)) {
            originalResizeStop = p.resizeStop;
        }
        $firstRow = $theadInTable.find("tr.jqg-first-row-header");
        p.resizeStop = function (nw, idx) {
            var newWidth = this.newWidth;

            $firstRow.find("th:nth-child(" + (idx + 1) + ")").width(nw);

            // ajust the size of gview, gbox and the pager base on the width of htable
            $gview.width(newWidth);
            $gbox.width(newWidth);
            $bdiv.width(newWidth);
            $hdiv.width(newWidth);
            $gbox.children("div.ui-jqgrid-pager").width(newWidth);     // set pager width
            $gview.children("div.ui-jqgrid-toppager").width(newWidth); // set toppager width
            $gview.children("div.ui-jqgrid-sdiv").width(newWidth);     // set footer width

            if ($.isFunction(originalResizeStop)) {
                originalResizeStop.call(ts, nw, idx);
            }
        };
    };
});