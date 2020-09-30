import requests
import json

def test_get_nonexistent_comment():
  url = 'http://localhost:5300/comments/nonexistent-id'

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 404

def test_get_comment_by_id():
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

  comment_resp = requests.post(url, headers=headers, data=json.dumps(payload,indent=4))
  commentId = comment_resp.json()['id']

  # test
  resp_body = resp.json()
  url = 'http://localhost:5300/comments/' + commentId

  headers = {'Content-Type': 'application/json' }

  resp = requests.get(url, headers=headers)
  assert resp.status_code == 200