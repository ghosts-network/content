import requests
import json

def test_get_comment_by_publication():
  # create publication
  url = 'http://localhost:5300/publications'
  
  headers = {'Content-Type': 'application/json' }
  
  payload = {'content': 'My first publication #awesome'}

  resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  publicationId = resp.json()['id']

  # add first comment
  url = 'http://localhost:5300/comments'

  headers = {'Content-Type': 'application/json' }

  payload = {
    'publicationId': publicationId,
    'content': 'comment for the first publication'
  }

  requests.post(url, headers=headers, data=json.dumps(payload,indent=4))

  url = 'http://localhost:5300/comments/bypublication/' + publicationId

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 200

  resp_body = resp.json()
  assert len(resp_body) == 1
  assert 'id' in resp_body[0]
  assert resp_body[0]['publicationId'] == publicationId

def test_get_comment_by_nonexistent_publication():
  url = 'http://localhost:5300/comments/bypublication/nonexistent-id'

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 404
