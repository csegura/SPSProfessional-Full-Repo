
function OptionChange(senderPartialId) {
    var control = document.getElementById('Option' + senderPartialId);
    var selectedValue = control.options[control.selectedIndex].value;
    var controlToChange = document.getElementById('RowOptionPanel' + senderPartialId);

    if (selectedValue == 'where')
        controlToChange.style.display = 'inline';
    else
        controlToChange.style.display = 'none';
} 

function OptionFieldWhereChange(senderPartialId) {
    var fieldControl = document.getElementById('OptionFieldWhere' + senderPartialId);
    var conditionControl = document.getElementById('OptionConditionWhere' + senderPartialId);
    var selectedValue = fieldControl.options[fieldControl.selectedIndex].value;

    if (selectedValue == '[Me]') {
        FillConditionForUser(conditionControl);
        document.getElementById('OptionValueFieldWhere' + senderPartialId).style.display = 'none';
        document.getElementById('OptionValueUserWhere' + senderPartialId).style.display = 'inline';
    }
    else {
        FillConditionForField(conditionControl);
        document.getElementById('OptionValueUserWhere' + senderPartialId).style.display = 'none';
        document.getElementById('OptionValueFieldWhere' + senderPartialId).style.display = 'inline';
    }
} 

function FillConditionForUser(conditionCtrl) {
    conditionCtrl.length = 2
    conditionCtrl.options[0].value = 'InGroup';
    conditionCtrl.options[0].text = 'Is in group';
    conditionCtrl.options[1].value = 'NotInGroup';
    conditionCtrl.options[1].text = 'Is not in group';
    conditionCtrl.selectedIndex = 0;
}

function FillConditionForField(conditionCtrl) {
    conditionCtrl.options.length = 9
    conditionCtrl.options[0].value = 'IsEqualTo';
    conditionCtrl.options[0].text = 'Is equal to';
    conditionCtrl.options[1].value = 'IsNotEqualTo';
    conditionCtrl.options[1].text = 'Is not equal to';
    conditionCtrl.options[2].value = 'IsGreaterThen';
    conditionCtrl.options[2].text = 'Is greater than';
    conditionCtrl.options[3].value = 'IsLessThan';
    conditionCtrl.options[3].text = 'Is less than';
    conditionCtrl.options[4].value = 'IsGreaterOrEqualThan';
    conditionCtrl.options[4].text = 'Is greater or equal than';
    conditionCtrl.options[5].value = 'IsLessOrEqualThan';
    conditionCtrl.options[5].text = 'Is less or equal than';
    conditionCtrl.options[6].value = 'BeginWith';
    conditionCtrl.options[6].text = 'Begin with';
    conditionCtrl.options[7].value = 'EndWith';
    conditionCtrl.options[7].text = 'End with';
    conditionCtrl.options[8].value = 'Contains';
    conditionCtrl.options[8].text = 'Contains';
    conditionCtrl.selectedIndex = 0;
} 

function ComputeField(senderPartialId) {
    var hiddenField = document.getElementById('Hidden' + senderPartialId);
    var option = document.getElementById('Option' + senderPartialId);
    var optionFieldWhere = document.getElementById('OptionFieldWhere' + senderPartialId);
    var optionConditionWhere = document.getElementById('OptionConditionWhere' + senderPartialId);
    var optionValueUserWhere = document.getElementById('OptionValueUserWhere' + senderPartialId);
    var optionValueFieldWhere = document.getElementById('OptionValueFieldWhere' + senderPartialId);

    var optionValue = option.options[option.selectedIndex].value;
    var optionFieldWhereValue = optionFieldWhere.options[optionFieldWhere.selectedIndex].value;
    var optionConditionWhereValue = optionConditionWhere.options[optionConditionWhere.selectedIndex].value;
    var optionValueUserWhereValue = optionValueUserWhere.options[optionValueUserWhere.selectedIndex].value;
    var optionValueFieldWhereValue = optionValueFieldWhere.value;

    hiddenField.value = optionValue + ';' + optionFieldWhereValue + ';' + optionConditionWhereValue + ';' + optionValueUserWhereValue + ';' + optionValueFieldWhereValue;
}

function OptionChange(id, sender) {
    if (CountChecked(id) == 0)
        document.getElementById(sender).checked = true;
    else
        OptionBind(id);
} 

function CountChecked(id) {
    var result = 0;
    var panel = document.getElementById('Div' + id);

    for (index = 0; index < panel.childNodes.length; index++) {
        var ctrl = panel.childNodes.item(index);
        if ((ctrl.name != null) && (ctrl.checked))
            result++;
    }

    return result;
} 

function OptionBind(id) {
    var selectCtrl = document.getElementById('Option' + id);
    var panel = document.getElementById('Div' + id);
    var i = 0;
    var lenght = CountChecked(id);

    selectCtrl.options.length = lenght;

    for (index = 0; index < panel.childNodes.length; index++) {
        var ctrl = panel.childNodes.item(index);
        if ((ctrl.name != null) && (ctrl.checked)) {
            selectCtrl.options[i].value = ctrl.value;
            selectCtrl.options[i].text = ctrl.title;
            i++;
        }
    }

    selectCtrl.selectedIndex = 0;
    ComputeHidden(id);
} 

function ComputeHidden(id) {
    var selectCtrl = document.getElementById('Option' + id);
    var panel = document.getElementById('Div' + id);
    var hidden = document.getElementById('Hidden' + id);
    var groupId = id;
    var defaultView = selectCtrl.options[selectCtrl.selectedIndex].value
    var _views = '';

    for (index = 0; index < panel.childNodes.length; index++) {
        var ctrl = panel.childNodes.item(index);
        if ((ctrl.name != null) && (ctrl.checked)) {
            _views = _views + ctrl.value + ';'
        }
    }

    hidden.value = groupId + '#' + defaultView + '#' + _views;
}