// TODO: move module into a separate file. With this common decorator.
angular.module("transactionsApp", ["ngResource", "ngTable"])
    // Format negative values as -$x.yy instead of ($x.yy)
    // http://stackoverflow.com/a/30122327/210780
    .config(['$provide', function($provide) {
        $provide.decorator('$locale', ['$delegate', function($delegate) {
            if($delegate.id == 'en-us') {
                $delegate.NUMBER_FORMATS.PATTERNS[1].negPre = '-\u00A4';
                $delegate.NUMBER_FORMATS.PATTERNS[1].negSuf = '';
            }
            return $delegate;
        }]);
    }])

    .controller('TransactionsController', function($resource, NgTableParams, $location) {
        var self = this;
        var transactions = [];

        // Get the account ID out of the URL
        var url = $location.absUrl();
        var lastSlash = url.lastIndexOf('/');
        var accountId = url.substring(lastSlash + 1);

        $resource('../../Transaction/GetAll', { AccountId: accountId }).query().$promise.then(function(results) {
            self.transactions = results;
            self.tableParams = new NgTableParams({}, { dataset: self.transactions });

            var lastMonthDate = new Date();
            lastMonthDate.setDate(1);
            lastMonthDate.setMonth(lastMonthDate.getMonth() - 1);

            self.lastMonth = { "name": lastMonthDate.toLocaleString('en-US', { month: "long" }), "totalIncome": 0, "totalExpenses": 0 };

            for (var i = 0; i < self.transactions.length; i++)
            {
                var transaction = self.transactions[i];
                var date = Date.fromString(transaction.TransactionDateUtc);
                if (date.getUTCMonth() == lastMonthDate.getUTCMonth() && date.year == lastMonthDate.year)
                {
                    if (transaction.Amount > 0)
                    {
                        self.lastMonth.totalIncome += transaction.Amount;
                    }
                    else
                    {
                        // Get a positive number
                        self.lastMonth.totalExpenses += -1 * transaction.Amount;
                    }
                }
            }
        });
    });