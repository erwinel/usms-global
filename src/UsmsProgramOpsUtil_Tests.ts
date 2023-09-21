/// <reference path="../types/index.d.ts" />
/// <reference path="../types/sn_atf.d.ts" />
/// <reference path="../types/jasmine.d.ts" />
/// <reference path="x_g_doj_usmsprogop.d.ts" />
/// <reference path="UsmsProgramOpsUtil.d.ts" />

namespace UsmsProgramOpsUtil_Tests {
    declare function steps(sys_id: string): sn_atf.ITestStepOutputs;
    declare function assertEqual(assertion: sn_atf.ITestAssertion): void;
    declare var outputs: sn_atf.ITestStepOutputs;
    declare var stepResult: sn_atf.ITestStepResult;
    
    // Step 1: normalizeWhitespace
    (function normalizeWhitespaceTest(outputs: sn_atf.ITestStepOutputs, steps: sn_atf.ITestStepsFunc, stepResult: sn_atf.ITestStepResult, assertEqual: sn_atf.IAssertEqualFunc) {
        describe('UsmsProgramOpsUtil.normalizeWhitespace tests', function() {
            it('undefined returns undefined', function() {
                var expected = 'undefined';
                var value = undefined;
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value);
                expect(expected).toBe(typeof actual);
            });
            it('empty returns undefined', function() {
                var expected = 'undefined';
                var value = "";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value);
                expect(expected).toBe(typeof actual);
            });
            it('whitespace returns undefined', function() {
                var expected = 'undefined';
                var value = "\n\t";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value);
                expect(expected).toBe(typeof actual);
            });
            it('"abc" returns "abc"', function() {
                var expected = 'abc';
                var value = 'abc';
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value);
                expect(expected).toBe(actual);
            });
            it('"\\tabc\\nxyz " returns "abc xyz"', function() {
                var expected = 'abc xyz';
                var value = "\tabc\nxyz ";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value);
                expect(expected).toBe(actual);
            });
            it('"\\tabc\\nxyz " with default returns "abc xyz"', function() {
                var expected = 'abc xyz';
                var value = "\tabc\nxyz ";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value, "pdq");
                expect(expected).toBe(actual);
            });
            it('undefined with default " pdq " returns " pdq "', function() {
                var expected = ' pdq ';
                var value = undefined;
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value, expected);
                expect(expected).toBe(actual);
            });
            it('empty with default " pdq " returns " pdq "', function() {
                var expected = ' pdq ';
                var value = "";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value, expected);
                expect(expected).toBe(actual);
            });
            it('whitespace with default "pdq" returns "pdq"', function() {
                var expected = 'pdq';
                var value = "\n\t";
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.normalizeWhitespace(value, expected);
                expect(expected).toBe(actual);
            });
        });
    })(outputs, steps, stepResult, assertEqual);
    jasmine.getEnv().execute();
    
    // Step 2: validateExpirationDate
    (function validateExpirationDateTest(outputs: sn_atf.ITestStepOutputs, steps: sn_atf.ITestStepsFunc, stepResult: sn_atf.ITestStepResult, assertEqual: sn_atf.IAssertEqualFunc) {
        describe('x_g_doj_usmsprogop.UsmsProgramOpsUtil.validateExpirationDate tests', function() {
            it('expiration = nil is true', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_group_access_assignment>new GlideRecord('x_g_doj_usmsprogop_u_cmdb_ci_group_access_assignment');
                gr.initialize();
                gr.setValue('u_effective_date', new GlideDate());
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validateExpirationDate(gr.u_effective_date, gr.u_expiration_date);
                expect(actual).toBe(true);
            });
            it('expiration = effective - 1 day is false', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_group_access_assignment>new GlideRecord('x_g_doj_usmsprogop_u_cmdb_ci_group_access_assignment');
                gr.initialize();
                var n = new GlideDate();
                n.addDays(1);
                gr.setValue('u_effective_date', n);
                gr.setValue('u_expiration_date', new GlideDate());
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validateExpirationDate(gr.u_effective_date, gr.u_expiration_date);
                expect(actual).toBe(false);
            });
            it('expiration = effective is true', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_group_access_assignment>new GlideRecord('x_g_doj_usmsprogop_u_cmdb_ci_group_access_assignment');
                gr.initialize();
                var n = new GlideDate();
                gr.setValue('u_effective_date', n);
                gr.setValue('u_expiration_date', n);
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validateExpirationDate(gr.u_effective_date, gr.u_expiration_date);
                expect(actual).toBe(true);
            });
            it('expiration = effective + 1 day is true', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_group_access_assignment>new GlideRecord('x_g_doj_usmsprogop_u_cmdb_ci_group_access_assignment');
                gr.initialize();
                var n = new GlideDate();
                n.addDays(1);
                gr.setValue('u_effective_date', new GlideDate());
                gr.setValue('u_expiration_date', n);
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validateExpirationDate(gr.u_effective_date, gr.u_expiration_date);
                expect(actual).toBe(true);
            });
        });
    })(outputs, steps, stepResult, assertEqual);
    jasmine.getEnv().execute();
    
    /*
        Step3: 90eee2e11bb0bd109a872f41f54bcb17
            Insert a record into 'x_g_doj_usmsprogop_u_cmdb_ci_physical_network' with the following values:
            Name = Test A
            and Display Order = 100

        Step 4: c32fe6e11bb0bd109a872f41f54bcb05
            Insert a record into 'x_g_doj_usmsprogop_u_cmdb_ci_physical_network' with the following values:
            Name = Test B
            and Display Order = 200
            and Restricted Transfer Networks = 90eee2e11bb0bd109a872f41f54bcb17 {{Step 3: Record Insert.Record}}

        Step 5: 47ef2ae11bb0bd109a872f41f54bcb1b
            Insert a record into 'x_g_doj_usmsprogop_u_cmdb_ci_physical_network' with the following values:
            Name = Test C
            and Restricted Transfer Networks = c32fe6e11bb0bd109a872f41f54bcb05 {{Step 4: Record Insert.Record}}
            and Display Order = 300

        Step 6: 3d91c9811b9d3d109a872f41f54bcbac
            Insert a record into 'x_g_doj_usmsprogop_u_cmdb_ci_physical_network' with the following values:
            Name = Test D
            and Restricted Transfer Networks = c32fe6e11bb0bd109a872f41f54bcb05 {{Step 4: Record Insert.Record}}
            and Operational status = Operational
    */

    // Step 7: validatePhysicalNetwork
    (function validatePhysicalNetworkTest(outputs: sn_atf.ITestStepOutputs, steps: sn_atf.ITestStepsFunc, stepResult: sn_atf.ITestStepResult, assertEqual: sn_atf.IAssertEqualFunc) {
        describe('x_g_doj_usmsprogop.UsmsProgramOpsUtil.validatePhysicalNetwork tests', function() {
            it('Setting circular u_restricted_transfer_networks reference should return false', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_physical_network>new GlideRecord("x_g_doj_usmsprogop_u_cmdb_ci_physical_network");
                var sys_id = '' + steps('90eee2e11bb0bd109a872f41f54bcb17').record_id;
                if (!gr.get(sys_id))
                    throw new Error("Unable to get record with sys_id " + JSON.stringify(sys_id));
                sys_id = '' + steps('3d91c9811b9d3d109a872f41f54bcbac').record_id;
                gr.setValue('u_restricted_transfer_networks', sys_id);
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validatePhysicalNetwork(gr);
                expect(actual).toBe(false);
            });
            it ('Setting u_restricted_transfer_networks the same as nested reference should return true', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_physical_network>new GlideRecord("x_g_doj_usmsprogop_u_cmdb_ci_physical_network");
                var sys_id = '' + steps('47ef2ae11bb0bd109a872f41f54bcb1b').record_id;
                if (!gr.get(sys_id))
                    throw new Error("Unable to get record with sys_id " + JSON.stringify(sys_id));
                gr.setValue('u_restricted_transfer_networks', steps('90eee2e11bb0bd109a872f41f54bcb17').record_id + ',' + steps('c32fe6e11bb0bd109a872f41f54bcb05').record_id);
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validatePhysicalNetwork(gr);
                if (!expect(actual).toBe(true))
                    gs.warn("Field error: " + JSON.stringify(gr.u_restricted_transfer_networks.getError()));
            });
            it('Cleared u_restricted_transfer_networks should return true', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_physical_network>new GlideRecord("x_g_doj_usmsprogop_u_cmdb_ci_physical_network");
                var sys_id = '' + steps('47ef2ae11bb0bd109a872f41f54bcb1b').record_id;
                if (!gr.get(sys_id))
                    throw new Error("Unable to get record with sys_id " + JSON.stringify(sys_id));
                gr.setValue('u_restricted_transfer_networks', '');
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validatePhysicalNetwork(gr);
                if (!expect(actual).toBe(true))
                    gs.warn("Field error: " + JSON.stringify(gr.u_restricted_transfer_networks.getError()));
            });
            it('Adding duplicate u_restricted_transfer_networks values should return removed SysIDs', function() {
                var gr = <x_g_doj_usmsprogop.$$record.cmdb_ci_physical_network>new GlideRecord("x_g_doj_usmsprogop_u_cmdb_ci_physical_network");
                var sys_id = '' + steps('47ef2ae11bb0bd109a872f41f54bcb1b').record_id;
                if (!gr.get(sys_id))
                    throw new Error("Unable to get record with sys_id " + JSON.stringify(sys_id));
                var ref_id = '' + steps('90eee2e11bb0bd109a872f41f54bcb17').record_id;
                gr.setValue('u_restricted_transfer_networks', ref_id + ',' + ref_id + ',' + ref_id);
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.validatePhysicalNetwork(gr);
                expect(typeof actual).toBe('object');
                expect(Array.isArray(actual)).toBe(true);
                expect((<string[]>actual).length).toBe(2);
                expect((<string[]>actual)[0]).toBe(ref_id);
                expect((<string[]>actual)[1]).toBe(ref_id);
                expect(gr.getValue('u_restricted_transfer_networks')).toBe(ref_id);
            });
        });
    })(outputs, steps, stepResult, assertEqual);
    jasmine.getEnv().execute();

    // Step 8: toCommaSeparatedPhrase
    (function toCommaSeparatedPhraseTest(outputs: sn_atf.ITestStepOutputs, steps: sn_atf.ITestStepsFunc, stepResult: sn_atf.ITestStepResult, assertEqual: sn_atf.IAssertEqualFunc) {
        describe('x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase tests', function() {
            it('Empty array should return empty string', function() {
                var arr: string[] = [];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr);
                expect(actual).toBe("");
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "and");
                expect(actual).toBe("");
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "or");
                expect(actual).toBe("");
            });
            it('Single-element array should return first element', function() {
                var arr: string[] = ["Test"];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr);
                expect(actual).toBe(arr[0]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "and");
                expect(actual).toBe(arr[0]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "or");
                expect(actual).toBe(arr[0]);
            });
            it('Two-element array should have no comma', function() {
                var arr: string[] = ["Test", "Value"];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr);
                expect(actual).toBe(arr[0] + " and " + arr[1]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "and");
                expect(actual).toBe(arr[0] + " and " + arr[1]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "AND");
                expect(actual).toBe(arr[0] + " AND " + arr[1]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "or");
                expect(actual).toBe(arr[0] + " or " + arr[1]);
            });
            it ('Multiple element array should return comma-separated with last element preceded by conjunction', function() {
                var arr: string[] = ["Test", "Join", "Value"];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr);
                expect(actual).toBe(arr[0] + ", " + arr[1] + ", and " + arr[2]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "and");
                expect(actual).toBe(arr[0] + ", " + arr[1] + ", and " + arr[2]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "AND");
                expect(actual).toBe(arr[0] + ", " + arr[1] + ", AND " + arr[2]);
                actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.toCommaSeparatedPhrase(arr, "or");
                expect(actual).toBe(arr[0] + ", " + arr[1] + ", or " + arr[2]);
            });
        });
    })(outputs, steps, stepResult, assertEqual);
    jasmine.getEnv().execute();

    // Step 9: copyArray
    (function copyArrayTest(outputs: sn_atf.ITestStepOutputs, steps: sn_atf.ITestStepsFunc, stepResult: sn_atf.ITestStepResult, assertEqual: sn_atf.IAssertEqualFunc) {
        describe('x_g_doj_usmsprogop.UsmsProgramOpsUtil.copyArray tests', function() {
            it('Empty array should return a new empty array', function() {
                var arr: string[] = [];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.copyArray(arr);
                expect(typeof actual).toBe('object');
                expect(actual).not.toBeNull();
                expect(Array.isArray(actual)).toBe(true);
                expect(actual.length).toBe(0);
                arr.push('Value');
                expect(actual.length).toBe(0);
            });
            it('Single-element array should return new array with same first element', function() {
                var arr: string[] = ["Test"];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.copyArray(arr);
                expect(typeof actual).toBe('object');
                expect(actual).not.toBeNull();
                expect(Array.isArray(actual)).toBe(true);
                expect(actual.length).toBe(1);
                expect(actual[0]).toBe(arr[0]);
                arr.push('Value');
                expect(actual.length).toBe(1);
                expect(actual[0]).toBe(arr[0]);
            });
            it('Multi-element array should return new array with equal elements', function() {
                var arr: string[] = ["Test", "Array", "Value"];
                var actual = x_g_doj_usmsprogop.UsmsProgramOpsUtil.copyArray(arr);
                expect(typeof actual).toBe('object');
                expect(actual).not.toBeNull();
                expect(Array.isArray(actual)).toBe(true);
                expect(actual.length).toBe(3);
                expect(actual[0]).toBe(arr[0]);
                expect(actual[1]).toBe(arr[1]);
                expect(actual[2]).toBe(arr[2]);
                arr.push('Added');
                expect(actual.length).toBe(3);
                expect(actual[0]).toBe(arr[0]);
                expect(actual[1]).toBe(arr[1]);
                expect(actual[2]).toBe(arr[2]);
            });
        });
    })(outputs, steps, stepResult, assertEqual);
    jasmine.getEnv().execute();
}
