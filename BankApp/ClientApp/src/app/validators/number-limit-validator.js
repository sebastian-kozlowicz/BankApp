"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var NumberLimitValidator = /** @class */ (function () {
    function NumberLimitValidator() {
    }
    NumberLimitValidator.limitValidator = function (min, max, error) {
        return function (control) {
            if (control.value && (isNaN(control.value) || control.value < min || control.value > max))
                return error;
            return null;
        };
    };
    return NumberLimitValidator;
}());
exports.NumberLimitValidator = NumberLimitValidator;
//# sourceMappingURL=number-limit-validator.js.map