import http from 'k6/http';

export default function() {
  var url = 'http://localhost:5300/publications';
  var payload = JSON.stringify({ content: 'Very goood publication!!!' });

  var params = { headers: { 'Content-Type': 'application/json' } };

  http.post(url, payload, params);
}
