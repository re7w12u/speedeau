

//------------------------ NOTIFICATION ---------------------------------
RNVO.notify = function () { }
RNVO.notify.delay = '15000'; // 15 seconds

RNVO.notify.log = function (msg) {
    RNVO.notify.cleanUp();
    statusId = SP.UI.Status.addStatus(msg);
    SP.UI.Status.setStatusPriColor(statusId, 'green');
    //RNVO.notify.setTimeOut();
}

RNVO.notify.error = function (msg) {
    RNVO.notify.cleanUp();
    statusId = SP.UI.Status.addStatus(msg);
    SP.UI.Status.setStatusPriColor(statusId, 'red');
    //RNVO.notify.setTimeOut();
}

RNVO.notify.warn = function (msg) {
    RNVO.notify.cleanUp();
    statusId = SP.UI.Status.addStatus(msg);
    SP.UI.Status.setStatusPriColor(statusId, 'Yellow');
    //RNVO.notify.setTimeOut();
}

RNVO.notify.cleanUp = function () {
    SP.UI.Status.removeAllStatus();
}

RNVO.notify.setTimeOut = function () {
    setTimeout(RNVO.notify.cleanUp, RNVO.notify.delay);
}

RNVO.notify.note = function (msg) {
    SP.UI.Notify.addNotification(msg, false);
}

RNVO.notify.exception = function (sender, args) {
    RNVO.notify.error('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
}
RNVO.notify.registerClass("RNVO.notify");