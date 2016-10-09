    function initialiseTables(tableIds){
        for (var i = 0; i < tableIds.length; i++) {
            $(tableIds[i]).DataTable({ searching: false, info: false, ordering: false });
        }
    }