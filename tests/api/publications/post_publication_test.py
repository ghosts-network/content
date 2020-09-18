import requests
import json

def test_post_publication():
  url = 'http://localhost:5300/publications'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {'content': 'My first publication #awesome'}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  assert resp.status_code == 201

  resp_body = resp.json()
  assert 'id' in resp_body
  assert 'tags' in resp_body
  assert resp_body['tags'][0] == 'awesome'

def test_empty_body_publication():
  url = 'http://localhost:5300/publications'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  assert resp.status_code == 400

  resp_body = resp.json()
  assert 'Content' in resp_body['errors']
