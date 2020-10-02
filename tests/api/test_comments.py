from src.api import Api

class TestComments(Api):
  def test_get_nonexistent_comment(self):
    resp = self.get_comment_by_id('nonexistent-id')

    assert resp.status_code == 404
  
  def test_get_comment_by_id(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publicationId = publication_resp.json()['id']

    comment_resp = self.post_comment({'publicationId': publicationId, 'content': 'comment for the first publication'})
    commentId = comment_resp.json()['id']

    resp = self.get_comment_by_id(commentId)
    resp_body = resp.json()

    assert resp.status_code == 200

  def test_get_comments_by_publication(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publicationId = publication_resp.json()['id']

    comment_resp = self.post_comment({'publicationId': publicationId, 'content': 'comment for the first publication'})
    commentId = comment_resp.json()['id']
    
    resp = self.get_comments_by_publication_id(publicationId)
    resp_body = resp.json()

    assert resp.status_code == 200
    assert len(resp_body) == 1
    assert resp_body[0]['id'] == commentId
    assert resp_body[0]['publicationId'] == publicationId
    assert resp_body[0]['content'] == 'comment for the first publication'

  def test_get_comment_by_nonexistent_publication(self):
    resp = self.get_comments_by_publication_id('nonexistent-id')

    assert resp.status_code == 404

  def test_post_comment(self):
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publicationId = publication_resp.json()['id']

    resp = self.post_comment({'publicationId': publicationId, 'content': 'comment for the first publication'})
    resp_body = resp.json()

    assert resp.status_code == 201
    assert 'id' in resp_body

  def test_empty_body_comment(self):
    resp = self.post_comment({})
    resp_body = resp.json()

    assert resp.status_code == 400
    assert 'Content' in resp_body['errors']