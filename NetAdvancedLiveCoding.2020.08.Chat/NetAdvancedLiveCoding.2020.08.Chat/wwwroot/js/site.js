let index = 0;
const host = '/home/';

if (!localStorage.userId)
    localStorage.userId = Math.round(Math.random() * 100000 + 1);


function showMessage(message) {
    const incomingMessageTemplate = `<div class="incoming_msg">
  <div class="incoming_msg_img"> <img src="https://ptetutorials.com/images/user-profile.png" alt="sunil"> </div>
  <div class="received_msg">
      <div class="received_withd_msg">
          <p>%text%</p>
          <span class="time_date">%time% | %date%</span>
      </div>
  </div>
</div>`
    const outgoingMessageTemplate = `<div class="outgoing_msg">
  <div class="sent_msg">
      <p>%text%</p>
      <span class="time_date">%time% | %date%</span>
  </div>
</div>`
    let msgHtml = message.userId == +localStorage.userId ? outgoingMessageTemplate : incomingMessageTemplate;

    msgHtml = msgHtml
        .replace('%text%', message.text)
        .replace('%time%', message.time)
        .replace('%date%', message.date);
    const target = $('.msg_history');
    target.append(msgHtml).animate({ scrollTop: target.height() }, 100);
    index++;
}

function listen() {
    $.get(host + 'listen?index=' + index, function (data) {
        for (let i = 0; i < data.length; i++)
            showMessage(data[i]);
        listen();
    });
}

$(document).ready(function () {
    listen();
    $('.msg_send_btn').click(function () {
        $.post(host + 'send', {
            userId: localStorage.userId,
            text: $('.write_msg').val()
        }, function () {
            $('.write_msg').val('');
        });
    })
});